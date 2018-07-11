using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.FileUploadModels;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Anlab.Core.Domain;
using Anlab.Core.Extensions;
using Anlab.Core.Services;
using AnlabMvc.Extensions;
using Serilog;
using AnlabMvc.Helpers;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.LabUser)]
    public class LabController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly ILabworksService _labworksService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISlothService _slothService;

        private const int _maxShownOrders = 1000;

        public LabController(ApplicationDbContext dbContext, IOrderService orderService, ILabworksService labworksService, IOrderMessageService orderMessageService, IFileStorageService fileStorageService, ISlothService slothService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
            _labworksService = labworksService;
            _orderMessageService = orderMessageService;
            _fileStorageService = fileStorageService;
            _slothService = slothService;
        }

        [HttpGet]
        public IActionResult Orders(bool showComplete)
        {
            var ordersQueryable = _dbContext.Orders
                    .Where(a => a.Status != OrderStatusCodes.Created);
            if (!showComplete)
            {
                ordersQueryable = ordersQueryable.Where(a => a.Status != OrderStatusCodes.Complete);

            }

            var orders = ordersQueryable.Select(c => new Order
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    Creator = new User { Email = c.Creator.Email },
                    Created = c.Created,
                    Updated = c.Updated,
                    RequestNum = c.RequestNum,
                    Status = c.Status,
                    ShareIdentifier = c.ShareIdentifier,
                    Paid = c.Paid,
                    ClientName = c.ClientName
                })
            .OrderByDescending(a => a.Updated)
            .Take(_maxShownOrders)
            .ToList();

            ViewBag.ShowComplete = showComplete;

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            return View(model);
        }

        public async Task<IActionResult> AddRequestNumber(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed) 
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> AddRequestNumber(int id, bool confirm, string requestNum)
        {
            if (String.IsNullOrWhiteSpace(requestNum))
            {
                ErrorMessage = "A request number is required";
                return RedirectToAction("AddRequestNumber");
            }

            requestNum = requestNum.SafeToUpper();  //Force Uppercase

            var checkReqNum = await _dbContext.Orders.AnyAsync(i => i.Id != id && i.RequestNum == requestNum);
            if (checkReqNum)
            {
                ErrorMessage = "That request number is already in use";
#if !DEBUG
                return RedirectToAction("AddRequestNumber");
#endif
            }

            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }


            order.RequestNum = requestNum;
            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage =
                        string.Format("Error. Unable to continue. The following codes were not found locally: {0}",
                            string.Join(",", result.MissingCodes));
                }

                return RedirectToAction("Orders");
            }

            if (!string.IsNullOrWhiteSpace(order.ClientId) && order.ClientId != result.ClientId)
            {
                ErrorMessage = $"Warning!!! Client Id is changing from {order.ClientId} to {result.ClientId}";
            }

            order = await UpdateOrderFromLabworksResult(order, result);

            await _dbContext.SaveChangesAsync();

            Message = "Order updated from work request number";
            return RedirectToAction("Confirmation", new { id = id });

        }

        /// <summary>
        /// This should probably be moved to a service...
        /// </summary>
        /// <param name="order"></param>
        /// <param name="result"></param>
        /// <param name="finalizeModel"></param>
        /// <returns></returns>
        private async Task<Order> UpdateOrderFromLabworksResult(Order order, OverwriteOrderResult result, LabFinalizeModel finalizeModel = null)
        {
            order.ClientId = result.ClientId;
            order.ClientName = "[Not Found]"; //Updated below if we find it
            var orderDetails = order.GetOrderDetails();

            if (!string.IsNullOrWhiteSpace(order.ClientId))
            {

                //TODO: update other info if we pull it: phone numbers
                var clientInfo = await _labworksService.GetClientDetails(order.ClientId);

                if (clientInfo != null)
                {
                    orderDetails.ClientInfo.ClientId = order.ClientId;
                    orderDetails.ClientInfo.Email = clientInfo.SubEmail;
                    orderDetails.ClientInfo.Name = clientInfo.Name;
                    orderDetails.ClientInfo.PhoneNumber = clientInfo.SubPhone;
                    orderDetails.ClientInfo.CopyPhone = clientInfo.CopyPhone;
                    orderDetails.ClientInfo.Department = clientInfo.Department;
                    order.ClientName = clientInfo.Name;
                }

                order.AdditionalEmails = AdditionalEmailsHelper.AddClientInfoEmails(order, orderDetails.ClientInfo);

            }

            orderDetails.Quantity = result.Quantity;
            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.IsInternalClient ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);
            orderDetails.Total = orderDetails.Total * result.RushMultiplier;
            orderDetails.RushMultiplier = result.RushMultiplier;
            orderDetails.LabworksSampleDisposition = result.LabworksSampleDisposition;

            if (finalizeModel != null)
            {
                orderDetails.LabComments = finalizeModel.LabComments;
                orderDetails.AdjustmentAmount = finalizeModel.AdjustmentAmount;
            }

            order.SaveDetails(orderDetails);
            order.SaveBackedupTestDetails(result.BackedupTests);

            return order;
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(int id, LabReceiveModel model)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            if(String.IsNullOrWhiteSpace(order.RequestNum))
            {
                ErrorMessage = "You must add a request number first";
                return RedirectToAction("AddRequestNumber", new { id = id });
            }

            var orderDetails = order.GetOrderDetails();

            orderDetails.LabComments = model.LabComments;
            orderDetails.AdjustmentAmount = model.AdjustmentAmount;

            order.SaveDetails(orderDetails);

            order.Status = OrderStatusCodes.Received;

            await _orderService.SendOrderToAnlab(order);

            await _orderMessageService.EnqueueReceivedMessage(order, model.BypassEmail);

            await _dbContext.SaveChangesAsync();

            Message = "Order marked as received";
            return RedirectToAction("Orders");

        }

        public async Task<IActionResult> Finalize(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Received)
            {
                ErrorMessage = "You can only Complete a Received order";
                return RedirectToAction("Orders");
            }

            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                }
                return RedirectToAction("Orders");
            }

            order = await UpdateOrderFromLabworksResult(order, result);

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Finalize(int id, LabFinalizeModel model)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Received)
            {
                ErrorMessage = "You can only Complete a Received order";
                return RedirectToAction("Orders");
            }

            if (model.UploadFile == null || model.UploadFile.Length <= 0)
            {
                ErrorMessage = "You need to upload the results at this time.";
                return RedirectToAction("Finalize", new{id});
            }

            order.Status = OrderStatusCodes.Finalized;

            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                }
                return RedirectToAction("Orders");
            }

            //File Upload
            order.ResultsFileIdentifier = await _fileStorageService.UploadFile(model.UploadFile);

            order = await UpdateOrderFromLabworksResult(order, result, model);
            
            var extraMessage = string.Empty;
            if (order.PaymentType == PaymentTypeCodes.UcDavisAccount)
            {
                var slothResult = await _slothService.MoveMoney(order);
                if (slothResult.Success)
                {
                    order.KfsTrackingNumber = slothResult.KfsTrackingNumber;
                    order.SlothTransactionId = slothResult.Id;
                    order.Paid = true;
                    extraMessage = " and UC Davis account marked as paid";
                }
                else
                {
                    ErrorMessage = $"There was a problem processing the payment for this account. {slothResult.Message}";
                    return RedirectToAction("Finalize");
                }
            }
            //Only send email if there wasn't a problem with sloth.
            await _orderMessageService.EnqueueFinalizedMessage(order, model.BypassEmail);
            await _dbContext.SaveChangesAsync();

            Message = $"Order marked as Finalized{extraMessage}";
            return RedirectToAction("Orders");

        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpGet]
        public async Task<ActionResult> OverrideOrder(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OverrideOrderModel
            {
                OrderReviewModel =
                {
                    Order = order,
                    OrderDetails = order.GetOrderDetails(),
                    HideLabDetails = false
                },
                IsDeleted = order.IsDeleted,
                Paid = order.Paid,
                Status = order.Status,
                Emails = order.AdditionalEmails
            };

            return View(model);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpPost]
        public async Task<ActionResult> OverrideOrder(int id, OverrideOrderModel model)
        {
            var orderToUpdate = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (orderToUpdate == null)
            {
                return NotFound();
            }

            if (orderToUpdate.Status != model.Status)
            {
                if (!OrderStatusCodes.All.Contains(model.Status))
                {
                    ErrorMessage = $"Unexpected Status Value: {model.Status}";
                    return RedirectToAction("OverrideOrder", new {id});
                }
            }

            //TODO: Parse emails to validate
            if (orderToUpdate.AdditionalEmails != model.Emails)
            {
                if (string.IsNullOrWhiteSpace(model.Emails))
                {
                    orderToUpdate.AdditionalEmails = string.Empty;
                }
                else
                {
                    var filteredEmailList = new List<string>();
                    var emailList = model.Emails.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var em in emailList)
                    {
                        var em1 = em.ToLower().Trim();
                        if (em1.IsEmailValid())
                        {
                            if (!filteredEmailList.Contains(em1))
                            {
                                filteredEmailList.Add(em1);
                            }
                        }
                        else
                        {
                            ErrorMessage = $"Invalid email detected: {em1}";
                            return RedirectToAction("OverrideOrder", new { id });
                        }

                    }

                    orderToUpdate.AdditionalEmails = string.Join(';', filteredEmailList);
                }
            }

            orderToUpdate.Paid = model.Paid;
            orderToUpdate.Status = model.Status;
            orderToUpdate.IsDeleted = model.IsDeleted;
            if (model.UploadFile != null && model.UploadFile.Length >= 0)
            {
                Log.Information($"Old Results File Identifier {orderToUpdate.ResultsFileIdentifier}");
                orderToUpdate.ResultsFileIdentifier = await _fileStorageService.UploadFile(model.UploadFile);
                Log.Information($"New Results File Identifier {orderToUpdate.ResultsFileIdentifier}");
            }

            await _dbContext.SaveChangesAsync();
            
            if (model.IsDeleted)
            {
                ErrorMessage = "Order deleted!!!";
                return RedirectToAction("Orders");
            }
            Message = "Order Updated";
            if (model.Status == OrderStatusCodes.Created)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", new{id});
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string term)
        {
            Order order = null;
            Guid guidSearch = Guid.Empty;
            int intSearch = 0;

            if (Guid.TryParse(term, out guidSearch))
            {
                order = await _dbContext.Orders.SingleOrDefaultAsync(a => a.ShareIdentifier == guidSearch);
            }
            else if(int.TryParse(term, out intSearch))
            {
                order = await _dbContext.Orders.SingleOrDefaultAsync(a => a.Id == intSearch);
            }
            else
            {
                order = await _dbContext.Orders.FirstOrDefaultAsync(a => a.RequestNum == term.SafeToUpper());
            }

            if (order == null)
            {
                ErrorMessage = "Order Not Found";
                return RedirectToAction("Search");
            }

            return RedirectToAction("Details", new {id = order.Id});

        }

        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<ActionResult> JsonDetails(int id)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return new JsonResult(order.GetOrderDetails());
        }

    }
}
