using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using AnlabMvc.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Controllers
{
    public class ResultsController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly IDataSigningService _dataSigningService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly AppSettings _appSettings;

        public ResultsController(ApplicationDbContext context,
            IFileStorageService fileStorageService,
            IDataSigningService dataSigningService,
            IOptions<CyberSourceSettings> cyberSourceSettings,
            IOptions<AppSettings> appSettings,
            IOrderMessageService orderMessageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _dataSigningService = dataSigningService;
            _orderMessageService = orderMessageService;
            _appSettings = appSettings.Value;
            _cyberSourceSettings = cyberSourceSettings.Value;
        }

        public async Task<IActionResult> Link(Guid id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.ShareIdentifier == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Finalized && order.Status != OrderStatusCodes.Complete)
            {
                return NotFound();
            }
            
            var model = new OrderResultsModel();
            
            model.OrderReviewModel.Order = order;
            model.OrderReviewModel.OrderDetails = order.GetOrderDetails();

            if (order.PaymentType == PaymentTypeCodes.CreditCard && !order.Paid)
            {
                model.ShowCreditCardPayment = true;
                Dictionary<string, string> dictionary = SetDictionaryValues(order, order.Creator);

                ViewBag.Signature = _dataSigningService.Sign(dictionary);
                model.PaymentDictionary = dictionary;
                model.CyberSourceUrl = _appSettings.CyberSourceUrl;
            }

            return View(model);
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.ShareIdentifier == id);
            if (order == null)
            {
                return NotFound();
            }

            var user = await _context.Users.SingleOrDefaultAsync(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                Action = "Download Results",
                Status = order.Status,
                ActorId = user?.NormalizedUserName,
                ActorName = user?.Name,
                JsonDetails = order.JsonDetails,
            });

            await _context.SaveChangesAsync();

            var result = await _fileStorageService.GetSharedAccessSignature(order.ResultsFileIdentifier);
            return Redirect(result.AccessUrl);
        }

        [HttpGet] //Maybe move to the Payment controller...
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.ShareIdentifier == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Paid)
            {
                ErrorMessage = "Payment has already been confirmed.";
                return RedirectToAction("Link", new {id });
            }

            if (order.PaymentType == PaymentTypeCodes.CreditCard)
            {
                ErrorMessage = "Order requires Other Payment type or UC Account, not a Credit Card Payment type.";
                return RedirectToAction("Link", new { id });
            }

            var model = new PaymentConfirmationModel
            {
                Order = order,
                OtherPaymentInfo = order.GetOrderDetails().OtherPaymentInfo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(Guid id, OtherPaymentInfo otherPaymentInfo) //Put in model
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.ShareIdentifier == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Paid)
            {
                ErrorMessage = "Payment has already been confirmed.";
                return RedirectToAction("Link", new {id = id});
            }

            if (order.PaymentType == PaymentTypeCodes.CreditCard)
            {
                ErrorMessage = "Order requires Other Payment type or UC Account, not a Credit Card Payment type.";
                return RedirectToAction("Link", new { id = id });
            }


            if (order.PaymentType == PaymentTypeCodes.Other && string.IsNullOrWhiteSpace(otherPaymentInfo.PoNum))
            {
                ModelState.AddModelError("OtherPaymentInfo.PoNum", "PO # is required");
            }

            otherPaymentInfo.PaymentType = order.GetOrderDetails().OtherPaymentInfo.PaymentType;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "There were errors trying to save that.";
                var model = new PaymentConfirmationModel
                {
                    Order = order,
                    OtherPaymentInfo = otherPaymentInfo
                };
                return View(model);
            }

            var orderDetails = order.GetOrderDetails();
            orderDetails.OtherPaymentInfo = otherPaymentInfo;

            order.SaveDetails(orderDetails);
            order.Paid = true;
            order.Status = OrderStatusCodes.Complete; //mark as paid and completed (yeah, completed)

            await _orderMessageService.EnqueueBillingMessage(order); //Send email to accountants

            var user = await _context.Users.SingleOrDefaultAsync(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                Action = "Confirmed Payment",
                Status = order.Status,
                ActorId = user?.NormalizedUserName,
                ActorName = user?.Name,
                JsonDetails = order.JsonDetails,
            });


            await _context.SaveChangesAsync();


            return RedirectToAction("Link", new { id = id });
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
