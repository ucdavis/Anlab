using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using AnlabMvc.Extensions;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class OrderController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly ILabworksService _labworksService;
        private readonly IFinancialService _financialService;
        private readonly AppSettings _appSettings;

        private const string processingCode = "PROC";

        public OrderController(ApplicationDbContext context, IOrderService orderService, IOrderMessageService orderMessageService, ILabworksService labworksService, IFinancialService financialService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _orderService = orderService;
            _orderMessageService = orderMessageService;
            _labworksService = labworksService;
            _financialService = financialService;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var joined = await _orderService.PopulateTestItemModel();
            var proc = await _labworksService.GetPrice(processingCode);

            var user = _context.Users.Single(a => a.Id == CurrentUserId);

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                InternalProcessingFee = Math.Ceiling(proc.Cost),
                ExternalProcessingFee = Math.Ceiling(proc.Cost * _appSettings.NonUcRate),
                Defaults = new OrderEditDefaults {
                    DefaultAccount = user.Account?.ToUpper(),
                    DefaultEmail = user.Email,
                    DefaultCompanyName = user.CompanyName,
                    DefaultAcAddr = user.BillingContactAddress,
                    DefaultAcEmail = user.BillingContactEmail,
                    DefaultAcName = user.BillingContactName,
                    DefaultAcPhone = user.BillingContactPhone,
                },
            };

            if (!string.IsNullOrWhiteSpace(user.ClientId))
            {
                //Has a default client id, so try to get defaults:
                var defaults = await _labworksService.GetClientDetails(user.ClientId);
                if (defaults != null)
                {
                    model.Defaults.DefaultAccount = model.Defaults.DefaultAccount ?? defaults.DefaultAccount;
                    model.Defaults.DefaultClientId = defaults.ClientId;
                    model.Defaults.DefaultClientIdName = defaults.Name;
                    model.Defaults.DefaultSubEmail = defaults.SubEmail;
                    model.Defaults.DefaultCopyEmail = defaults.CopyEmail;

                }
            }

            return View(model);
        }



        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null) {
                return NotFound();
            }
            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return RedirectToAction("Index");
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "You can't edit an order that has been confirmed.";
                return RedirectToAction("Index");
            }

            var joined = await _orderService.PopulateTestItemModel();
            var proc = await _labworksService.GetPrice(processingCode);

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                Order = order,
                InternalProcessingFee = Math.Ceiling(proc.Cost),
                ExternalProcessingFee = Math.Ceiling(proc.Cost * _appSettings.NonUcRate),
                Defaults = new OrderEditDefaults
                {
                    DefaultEmail = order.Creator.Email
                }
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Save(OrderSaveModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var result in ModelState.Values)
                {
                    foreach (var errs in result.Errors)
                    {
                        errors.Add(errs.ErrorMessage);
                    }
                }

                //TODO: A better way to return errors. Or maybe not, they shouldn't really ever happen.
                return Json(new { success = false, message = "There were problems with your order. Unable to save. Errors: " + string.Join("--", errors) });
            }

            var idForRedirection = 0;

            if (model.OrderId.HasValue)
            {
                var orderToUpdate = await _context.Orders.Include(i => i.Creator).SingleAsync(a => a.Id == model.OrderId.Value);
                if (orderToUpdate.CreatorId != CurrentUserId)
                {
                    return Json(new { success = false, message = "This is not your order." });
                }
                if (orderToUpdate.Status != OrderStatusCodes.Created)
                {
                    return Json(new { success = false, message = "This has been confirmed and may not be updated." });
                }

                var allTests = await _orderService.PopulateTestItemModel();
                orderToUpdate.SaveTestDetails(allTests);

                _orderService.PopulateOrder(model, orderToUpdate);

                idForRedirection = model.OrderId.Value;
                await _context.SaveChangesAsync();
            }
            else
            {
                var user = _context.Users.Single(a => a.Id == CurrentUserId);
                var order = new Order
                {
                    CreatorId = CurrentUserId,
                    Creator = user,
                    Status = OrderStatusCodes.Created,
                    ShareIdentifier = Guid.NewGuid(),
                };

                var allTests = await _orderService.PopulateTestItemModel();
                order.SaveTestDetails(allTests);

                _orderService.PopulateOrder(model, order);

                _context.Add(order);
                await _context.SaveChangesAsync();
                idForRedirection = order.Id;
            }


            return Json(new { success = true, id = idForRedirection });
        }


        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            if (order.Status == OrderStatusCodes.Created)
            {
                ErrorMessage = "Must confim order before viewing details.";
                return RedirectToAction("Index");
            }
            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            await _orderService.UpdateTestsAndPrices(order);

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(int id, bool confirm)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "Already confirmed";
                return RedirectToAction("Index");
            }

            if (order.PaymentType == PaymentTypeCodes.UcDavisAccount)
            {
                var orderDetails = order.GetOrderDetails();
                try
                {
                    orderDetails.Payment.AccountName = await _financialService.GetAccountName(orderDetails.Payment.Account);
                }
                catch
                {
                    orderDetails.Payment.AccountName = string.Empty;
                }
                order.SaveDetails(orderDetails);
                if (string.IsNullOrWhiteSpace(orderDetails.Payment.AccountName))
                {
                    await _context.SaveChangesAsync();
                    ErrorMessage = "Unable to verify UC Account number. Please edit your order and re-enter the UC account number. Then try again.";
                    return RedirectToAction("Confirmation", new {id = order.Id});
                }
            }
            if (order.PaymentType == PaymentTypeCodes.Other)
            {
                var orderDetails = order.GetOrderDetails();
                if (orderDetails.OtherPaymentInfo.PaymentType.Equals("Agreement", StringComparison.OrdinalIgnoreCase))
                {
                    await _orderMessageService.EnqueueBillingMessage(order, "Anlab Work Order Billing Info -- Agreement"); //TODO: Maybe a different one for an Agreement?
                }
            }

            await _orderService.UpdateTestsAndPrices(order);

            UpdateAdditionalInfo(order);
            order.Status = OrderStatusCodes.Confirmed;

            await _orderMessageService.EnqueueCreatedMessage(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmed", new { id = id });

        }

        public async Task<IActionResult> Confirmed(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "Can't delete confirmed orders.";
                return RedirectToAction("Index");
            }

            _context.Remove(order);
            await _context.SaveChangesAsync();

            Message = "Order deleted";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<ClientDetailsLookupModel> LookupClientId(string id)
        {
            var result = await _labworksService.GetClientDetails(id);
            if (result == null)
            {
                return null;
            }
            return result;
        }

        private void UpdateAdditionalInfo(Order order)
        {
            var orderDetails = order.GetOrderDetails();

            StringBuilder sb = new StringBuilder();

            if(!String.IsNullOrWhiteSpace(orderDetails.AdditionalInfo))
            {
                sb.AppendLine(orderDetails.AdditionalInfo);
            }

            if (orderDetails.SampleType == TestCategories.Plant)
            {
                sb.AppendFormat("{0}: {1}{2}", "Plant reporting basis", orderDetails.SampleTypeQuestions.PlantReportingBasis, Environment.NewLine);
            }

            //To do this now, we have to look at the selected tests...
            if (orderDetails.SampleType == TestCategories.Soil)
            {
                sb.AppendFormat("{0}: {1}{2}", "Soil is imported", orderDetails.SelectedTests.Any(a => a.Id == "SP-FOR" || a.Analysis.Equals("Imported Soil", StringComparison.InvariantCultureIgnoreCase)), Environment.NewLine);
            }

            if (orderDetails.SampleType == TestCategories.Water)
            {
                sb.AppendFormat("{0}: {1}{2}", "Water filtered", orderDetails.SampleTypeQuestions.WaterFiltered.ToYesNoString(), Environment.NewLine);
                sb.AppendFormat("{0}: {1} {2}{3}", "Water preservative added", orderDetails.SampleTypeQuestions.WaterPreservativeAdded.ToYesNoString(), orderDetails.SampleTypeQuestions.WaterPreservativeInfo, Environment.NewLine);
                sb.AppendFormat("{0}: {1}{2}", "Water reported in mg/L", orderDetails.SampleTypeQuestions.WaterReportedInMgL.ToYesNoString(), Environment.NewLine);
            }

            if (orderDetails.AdditionalInfoList != null)
            {
                foreach (var item in orderDetails.AdditionalInfoList)
                {
                    if (orderDetails.SelectedTests.Any(a => a.Id == item.Key))
                    {
                        sb.AppendFormat("{0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                }
                orderDetails.AdditionalInfoList = new Dictionary<string, string>();
            }

            orderDetails.AdditionalInfo = sb.ToString();

            order.SaveDetails(orderDetails);
        }
    }
   
}
