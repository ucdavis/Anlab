using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Models.CyberSource;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Newtonsoft.Json;

namespace AnlabMvc.Controllers
{
    public class PaymentController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderMessageService _orderMessageService;
        private readonly IDataSigningService _dataSigningService;
        private readonly AppSettings _appSettings;
        private readonly CyberSourceSettings _cyberSourceSettings;

        public PaymentController(ApplicationDbContext context, IOrderMessageService orderMessageService, IDataSigningService dataSigningService, IOptions<CyberSourceSettings> cyberSourceSettings, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _orderMessageService = orderMessageService;
            _dataSigningService = dataSigningService;
            _appSettings = appSettings.Value;
            _cyberSourceSettings = cyberSourceSettings.Value;
        }

        public ActionResult Pay(Guid id)
        {
            var order = _context.Orders.Include(i => i.Creator).SingleOrDefault(a => a.ShareIdentifier == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Accepted)
            {
                ErrorMessage = "You cannot Pay until the Order is Accepted."; 
                return RedirectToAction("Index", "Order");
            }

            Dictionary<string, string> dictionary = SetDictionaryValues(order, order.Creator);

            ViewBag.Signature = _dataSigningService.Sign(dictionary);
            ViewBag.PaymentDictionary = dictionary;
            ViewBag.CyberSourceUrl = _appSettings.CyberSourceUrl;

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult Receipt(ReceiptResponseModel response)
        {
            Log.ForContext("response", response, true).Information("Receipt response received");

            // check signature
            var dictionary = Request.Form.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
                ErrorMessage = "An error has occurred. Payment not processed. If you experience further problems, contact us.";
                return RedirectToAction("Index", "Home");
            }

            //var test = ProcessPaymentEvent(response, dictionary); //For testing locally, can enable this.
            //_context.SaveChanges();

            var order = _context.Orders.SingleOrDefault(a => a.Id == response.Req_Reference_Number);
            if (order == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                ErrorMessage = "Order for payment not found. Please contact technical support.";
                return NotFound(response.Req_Reference_Number);
            }
            //Note, don't check who has access as anyone may pay.
            
            var responseValid = CheckResponse(response);
            if (!responseValid.IsValid)
            {
                ErrorMessage = ErrorMessage = string.Format("Errors detected: {0}", string.Join(",", responseValid.Errors));
                return RedirectToAction("Pay", new {id = order.Id});
            }

            //Should be good,   
            Message = "Payment Processed. Thank You.";

            //ViewBag.PaymentDictionary = dictionary; //Debugging. Remove when not needed

            return View(response);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult Cancel(ReceiptResponseModel response)
        {
            {
                Log.ForContext("response", response, true).Information("Cancel response received");

                // check signature
                var dictionary = Request.Form.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
                if (!_dataSigningService.Check(dictionary, response.Signature))
                {
                    Log.Error("Check Signature Failure");
                    ErrorMessage = "An error has occurred. Payment not processed. If you experience further problems, contact us.";
                    return RedirectToAction("Index", "Home");
                }
                //ViewBag.PaymentDictionary = dictionary; //Debugging. Remove when not needed
                //ProcessPaymentEvent(response, dictionary); //TODO: Do we want to try to write cancel events?

                return View(response);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> ProviderNotify(ReceiptResponseModel response)
        {
            Log.ForContext("response", response, true).Information("Provider Notification Received");

            // check signature
            var dictionary = Request.Form.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
                return new JsonResult(new { });
            }

            var payment = ProcessPaymentEvent(response, dictionary);

            //Do payment stuff.
            var order = _context.Orders.SingleOrDefault(a => a.Id == response.Req_Reference_Number);
            if (order == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                return new JsonResult(new { });
            }

            if (response.Decision == ReplyCodes.Accept)
            {
                order.ApprovedPayment = payment;
                order.Paid = true;
                

                try
                {
                    await _orderMessageService.EnqueuePaidMessage(order);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }
            _context.SaveChanges();
            return new JsonResult(new { });
        }

        private PaymentEvent ProcessPaymentEvent(ReceiptResponseModel response, Dictionary<string, string> dictionary)
        {

            var paymentEvent = new PaymentEvent
            {
                Transaction_Id       = response.Transaction_Id,
                Auth_Amount          = response.Auth_Amount,
                Decision             = response.Decision,
                Reason_Code          = response.Reason_Code,
                Req_Reference_Number = response.Req_Reference_Number,
                ReturnedResults      = JsonConvert.SerializeObject(dictionary)
            };

            _context.PaymentEvents.Add(paymentEvent);

            return paymentEvent;
        }

        /// <summary>
        /// These are copied from Give and moved into a private method.
        /// They have only had basic testing done against them.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private CheckResponseResults CheckResponse(ReceiptResponseModel response)
        {
            var rtValue = new CheckResponseResults();
            //Ok, check response
            // general error, bad request
            if (string.Equals(response.Decision, ReplyCodes.Error) ||
                response.Reason_Code == ReasonCodes.BadRequestError ||
                response.Reason_Code == ReasonCodes.MerchantAccountError)
            {
                Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code)
                    .Warning("Unsuccessful Reply");
                rtValue.Errors.Add("An error has occurred. If you experience further problems, please contact us");
            }

            // this is only possible on a hosted payment page
            if (string.Equals(response.Decision, ReplyCodes.Cancel))
            {
                Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Warning("Cancelled Reply");
                rtValue.Errors.Add("The payment process was canceled before it could complete. If you experience further problems, please contact us");
            }

            // manual review required
            if (string.Equals(response.Decision, ReplyCodes.Review))
            {
                Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Warning("Manual Review Reply");
                rtValue.Errors.Add("Error with Credit Card. Please contact issuing bank. If you experience further problems, please contact us");
            }

            // bad cc information, return to payment page
            if (string.Equals(response.Decision, ReplyCodes.Decline))
            {
                if (response.Reason_Code == ReasonCodes.AvsFailure)
                {
                    Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Warning("Avs Failure");
                    rtValue.Errors.Add("We’re sorry, but it appears that the billing address that you entered does not match the billing address registered with your card. Please verify that the billing address and zip code you entered are the ones registered with your card issuer and try again. If you experience further problems, please contact us");
                }

                if (response.Reason_Code == ReasonCodes.BankTimeoutError ||
                    response.Reason_Code == ReasonCodes.ProcessorTimeoutError)
                {
                    Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Error("Bank Timeout Error");
                    rtValue.Errors.Add("Error contacting Credit Card issuing bank. Please wait a few minutes and try again. If you experience further problems, please contact us");
                }
                else
                {
                    Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Warning("Declined Card Error");
                    rtValue.Errors.Add("We’re sorry but your credit card was declined. Please use an alternative credit card and try submitting again. If you experience further problems, please contact us");
                }
            }

            // good cc info, partial payment
            if (string.Equals(response.Decision, ReplyCodes.Accept) &&
                response.Reason_Code == ReasonCodes.PartialApproveError)
            {
                //I Don't think this can happen.
                //TODO: credit card was partially billed. flag transaction for review
                //TODO: send to general error page
                Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code).Error("Partial Payment Error");
                rtValue.Errors.Add("We’re sorry but a Partial Payment Error was detected. Please contact us");
            }

            if (rtValue.Errors.Count <= 0)
            {
                if (response.Decision != ReplyCodes.Accept)
                {
                    Log.Error("Got past all the other checks. But it still wasn't Accepted");
                    rtValue.Errors.Add("Unknown Error. Please contact us.");
                }
                else
                {
                    rtValue.IsValid = true;
                }
            }
            return rtValue;
        }

        private class CheckResponseResults
        {
            public bool IsValid { get; set; } = false;
            public IList<string> Errors { get; set; } = new List<string>();
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