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

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class OrderController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly ILabworksService _labworksService;
        private readonly AppSettings _appSettings;

        public OrderController(ApplicationDbContext context, IOrderService orderService, IOrderMessageService orderMessageService, ILabworksService labworksService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _orderService = orderService;
            _orderMessageService = orderMessageService;
            _labworksService = labworksService;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var joined = await  _orderService.PopulateTestItemModel();
            var proc = await _labworksService.GetPrice("PROC");

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                InternalProcessingFee = Math.Ceiling(proc.Cost),
                ExternalProcessingFee = Math.Ceiling(proc.Cost * _appSettings.NonUcRate)
            };

            var user = _context.Users.Single(a => a.Id == CurrentUserId);
            model.DefaultAccount = user.Account;

            return View(model);
        }



        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o=>o.Id == id);

            if (order == null){
                return NotFound(id);
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

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                Order = order
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
                var orderToUpdate = await _context.Orders.SingleAsync(a => a.Id == model.OrderId.Value);
                if (orderToUpdate.CreatorId != CurrentUserId)
                {
                    return Json(new {success = false, message = "This is not your order."});
                }
                if(orderToUpdate.Status != OrderStatusCodes.Created)
                {
                    return Json(new { success = false, message = "This has been confirmed and may not be updated." });
                }

                await _orderService.PopulateOrder(model, orderToUpdate);

                idForRedirection = model.OrderId.Value;
                await _context.SaveChangesAsync();
            }
            else
            {
                var order = new Order
                {
                    CreatorId = CurrentUserId,
                    Status = OrderStatusCodes.Created,
                    ShareIdentifier = Guid.NewGuid(),
                };
                await _orderService.PopulateOrder(model, order);

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
                return NotFound(id);
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound(id);
            }

            if (order.Status == OrderStatusCodes.Created)
            {
                ErrorMessage = "Must confim order before viewing details.";
                return RedirectToAction("Index");
            }
            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.TestItems = _context.TestItems
                .Where(a => model.OrderDetails.SelectedTests.Select(s => s.Id).Contains(a.Id)).ToList();

            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound(id);
            }


            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.TestItems = _context.TestItems
                .Where(a => model.OrderDetails.SelectedTests.Select(s => s.Id).Contains(a.Id)).ToList();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(int id, bool confirm)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound(id);
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "Already confirmed";
                return RedirectToAction("Index");
            }

            order.Status = OrderStatusCodes.Confirmed;

            await _orderMessageService.EnqueueCreatedMessage(order);

            await _context.SaveChangesAsync();

            Message = "Order confirmed";
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound(id);
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
    }
   
}