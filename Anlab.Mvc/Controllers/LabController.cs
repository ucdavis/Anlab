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
using Anlab.Core.Services;
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
        [Obsolete]
        public IActionResult ListUsersOrders(string id)
        {
            var orders = _dbContext.Orders.Where(a => a.CreatorId == id && a.Status != OrderStatusCodes.Created).ToArray();
            return View(orders);
        }

        public async Task<IActionResult> AddRequestNumber(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

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

        [HttpPost]
        public async Task<IActionResult> AddRequestNumber(int id, bool confirm, string requestNum)
        {
            if (String.IsNullOrWhiteSpace(requestNum))
            {
                ErrorMessage = "A request number is required";
                return RedirectToAction("AddRequestNumber");
            }

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
            var result = await _orderService.OverwiteOrderFromDb(order);
            if (result.WasError)
            {
                ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                return RedirectToAction("Orders");
            }
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
                    orderDetails.ClientInfo.CopyEmail = clientInfo.CopyEmail;
                    orderDetails.ClientInfo.Name = clientInfo.Name;
                    order.ClientName = clientInfo.Name;
                }

                order.AdditionalEmails = AdditionalEmailsHelper.AddClientInfoEmails(order, orderDetails.ClientInfo);

            }

            orderDetails.Quantity = result.Quantity;
            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.ClientType == "uc" ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);
            orderDetails.Total = orderDetails.Total * result.RushMultiplier;
            orderDetails.RushMultiplier = result.RushMultiplier;
            
            order.SaveDetails(orderDetails);

            await _dbContext.SaveChangesAsync();

            Message = "Order updated from work request number";
            return RedirectToAction("Confirmation", new { id = id });

        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

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

            await _orderMessageService.EnqueueReceivedMessage(order);

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

            var result = await _orderService.OverwiteOrderFromDb(order);
            if (result.WasError)
            {
                ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                return RedirectToAction("Orders");
            }

            order.ClientId = result.ClientId;
            var orderDetails = order.GetOrderDetails();
            orderDetails.Quantity = result.Quantity;
            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.ClientType == "uc" ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);
            orderDetails.Total = orderDetails.Total * result.RushMultiplier;
            orderDetails.RushMultiplier = result.RushMultiplier;
            
            order.SaveDetails(orderDetails);

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
                //return RedirectToAction("Orders");
            }

            if (model.UploadFile == null || model.UploadFile.Length <= 0)
            {
                ErrorMessage = "You need to upload the results at this time.";
                return RedirectToAction("Finalize");
            }

            order.Status = OrderStatusCodes.Finalized;

            var result = await _orderService.OverwiteOrderFromDb(order);
            if (result.WasError)
            {
                ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                return RedirectToAction("Orders");
            }

            //File Upload
            order.ResultsFileIdentifier = await _fileStorageService.UploadFile(model.UploadFile);

            order.ClientId = result.ClientId;
            var orderDetails = order.GetOrderDetails();

            orderDetails.Quantity = result.Quantity;
            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.ClientType == "uc" ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);
            orderDetails.Total = orderDetails.Total * result.RushMultiplier;
            orderDetails.RushMultiplier = result.RushMultiplier;

            orderDetails.LabComments = model.LabComments;
            orderDetails.AdjustmentAmount = model.AdjustmentAmount;

            order.SaveDetails(orderDetails);

            await _orderMessageService.EnqueueFinalizedMessage(order);
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
                    ErrorMessage = "There was a problem processing the payment for this account.";
                    return RedirectToAction("Finalize");
                }
            }

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
                Status = order.Status
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
