using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using Anlab.Core.Data;
using Anlab.Core.Domain;
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

            if (order.Status != OrderStatusCodes.AwaitingPayment)
            {
                ErrorMessage = "You cannot Pay until the Order is ready."; 
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

            //ProcessPaymentEvent(response, dictionary); //For testing locally, can enable this.

            var order = _context.Orders.SingleOrDefault(a => a.Id == response.Req_Reference_Number);
            if (order == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                ErrorMessage = "Order for payment not found. Please contact technical support.";
                return NotFound(response.Req_Reference_Number);
            }
            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound(response.Req_Reference_Number);
            }

            var responeValid = CheckResponse(response);
            if (!responeValid.IsValid)
            {
                ErrorMessage = ErrorMessage = string.Format("Errors detected: {0}", string.Join(",", responeValid.Errors));
                return RedirectToAction("Pay", new {id = order.Id});
            }


            //Should be good, 
            if (response.Decision != ReplyCodes.Accept)
            {
                Log.Error("Got past all the other checks. But it still wasn't Accepted");
            }
            else
            {
                order.Status = OrderStatusCodes.Paid;
                _context.SaveChanges(true);
                Message = "Payment Processed. Thank You.";
            }



            //ViewBag.PaymentDictionary = dictionary; //Debugging. Remove when not needed

            return View(response);
        }

        private CheckResponseResults CheckResponse(ReceiptResponseModel response)
        {
            var rtValue = new CheckResponseResults();
            //Ok, check response
            // general error, bad request
            if (string.Equals(response.Decision, ReplyCodes.Error) ||
                response.Reason_Code == ReasonCodes.BadRequestError ||
                response.Reason_Code == ReasonCodes.MerchantAccountError)
            {
                //TODO: send to general error page
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
                //TODO: send to general error page
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
                rtValue.IsValid = true;
            }
            return rtValue;
        }

        private class CheckResponseResults
        {
            public bool IsValid { get; set; } = false;
            public IList<string> Errors { get; set; } = new List<string>();
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
                    ErrorMessage =
                        string.Format(
                            "An error has occurred. Payment not processed. If you experience further problems, contact us.");
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.PaymentDictionary = dictionary; //Debugging. Remove when not needed
                ProcessPaymentEvent(response, dictionary); //TODO: Do we want to try to write cancel events?

                return View(response);
            }
        }

        private void ProcessPaymentEvent(ReceiptResponseModel response, Dictionary<string, string> dictionary)
        {
            try
            {
                var paymentEvent = new PaymentEvent();
                paymentEvent.Transaction_Id = response.Transaction_Id;
                paymentEvent.Auth_Amount = response.Auth_Amount;
                paymentEvent.Decision = response.Decision;
                paymentEvent.Reason_Code = response.Reason_Code;
                paymentEvent.Req_Reference_Number = response.Req_Reference_Number;
                paymentEvent.ReturnedResults = JsonConvert.SerializeObject(dictionary);

                _context.PaymentEvents.Add(paymentEvent);
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                
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
            }
            else
            {
                ProcessPaymentEvent(response, dictionary);

                //Do payment stuff.
                var order = _context.Orders.SingleOrDefault(a => a.Id == response.Req_Reference_Number);
                if (order == null)
                {
                    Log.Error("Order not found {0}", response.Req_Reference_Number);
                }
                else
                {
                    if (response.Decision == ReplyCodes.Accept)
                    {
                        order.Status = OrderStatusCodes.Paid;
                        _context.SaveChanges(true);

                        try
                        {
                            await _orderMessageService.EnqueuePaidMessage(order);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, ex.Message);
                        }
                    }
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