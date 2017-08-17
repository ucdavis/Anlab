using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using Anlab.Core.Data;
using AnlabMvc.Models.CyberSource;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Newtonsoft.Json;

namespace AnlabMvc.Controllers
{
    public class PaymentController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataSigningService _dataSigningService;
        private readonly AppSettings _appSettings;
        private readonly CyberSourceSettings _cyberSourceSettings;

        public PaymentController(ApplicationDbContext context, IDataSigningService dataSigningService, IOptions<CyberSourceSettings> cyberSourceSettings, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _dataSigningService = dataSigningService;
            _appSettings = appSettings.Value;
            _cyberSourceSettings = cyberSourceSettings.Value;
        }

        public ActionResult Pay(int id)
        {
            var order = _context.Orders.Include(i => i.Creator).SingleOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }
            if (order.Creator == null)
            {
                return NotFound(CurrentUserId);
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return RedirectToAction("Index", "Order");
            }

            if (order.Status != OrderStatusCodes.Complete)
            {
                ErrorMessage = "You Pay until the Order is complete.";
                return RedirectToAction("Index", "Order");
            }

            Dictionary<string, string> dictionary = SetDictionaryValues(order, order.Creator);

            ViewBag.Signature = _dataSigningService.Sign(dictionary);
            ViewBag.PaymentDictionary = dictionary;
            ViewBag.CyberSourceUrl = _appSettings.CyberSourceUrl;

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.TestItems = _context.TestItems
                .Where(a => model.OrderDetails.SelectedTests.Select(s => s.Id).Contains(a.Id)).ToList();

            return View(model);
        }

        //public async Task<ActionResult> Receipt(ReceiptResponseModel response)
        //{
        //    Log.ForContext("response", response, true).Information("Receipt response received");

        //    // check signature
        //    var dictionary = Request.Form.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
        //    if (!_dataSigningService.Check(dictionary, response.Signature))
        //    {
        //        Log.Error("Check Signature Failure");
        //        TempData["ErrorMessage"] = string.Format("An error has occurred. If you experience further problems, contact us.");
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ProviderNotify(ReceiptResponseModel response)
        {
            Log.ForContext("response", response, true).Information("Provider Notification Received");

            // check signature
            var dictionary = Request.Form.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
            }
            else
            {
                //Do payment stuff.
                var order = _context.Orders.SingleOrDefault(a => a.Id == response.Req_Reference_Number);
                if (order == null)
                {
                    Log.Error("Order not found {0}", response.Req_Reference_Number);
                }
                else
                {
                    order.Status = "Paid"; //TODO, Flag or add to the list of codes.
                }
            }

            return new JsonResult(new { });
        }

        private Dictionary<string, string> SetDictionaryValues(Anlab.Core.Domain.Order order, Anlab.Core.Domain.User user)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("transaction_type", "sale");
            dictionary.Add("reference_number", order.Id.ToString());
            dictionary.Add("amount", order.GetOrderDetails().GrandTotal.ToString("F2"));
            dictionary.Add("currency", "USD");
            dictionary.Add("access_key", _cyberSourceSettings.AccessKey);
            dictionary.Add("profile_id", _cyberSourceSettings.ProfileId);
            dictionary.Add("transaction_uuid", Guid.NewGuid().ToString());
            dictionary.Add("signed_date_time", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
            dictionary.Add("unsigned_field_names", string.Empty);
            dictionary.Add("locale", "en");
            dictionary.Add("bill_to_email", user.Email);


            dictionary.Add("bill_to_forename", user.GetFirstName());
            dictionary.Add("bill_to_surname", user.GetLastName());

            dictionary.Add("bill_to_address_country", "US");
            dictionary.Add("bill_to_address_state", "CA");


            var fieldNames = string.Join(",", dictionary.Keys);
            dictionary.Add("signed_field_names", "signed_field_names," + fieldNames);
            return dictionary;
        }
    }
}