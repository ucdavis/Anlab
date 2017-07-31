using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.User)]
    public class LabController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly IOrderMessageService _orderMessageService;

        public LabController(ApplicationDbContext dbContext, IOrderService orderService, IOrderMessageService orderMessageService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
            _orderMessageService = orderMessageService;
        }

        public IActionResult OpenOrders()
        {
            //TODO: update this when we know status. Add filter?
            var orders = _dbContext.Orders.Where(a => a.Status != OrderStatusCodes.Created && a.Status != OrderStatusCodes.Complete)
                .Include(i => i.Creator).ToList();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

            if (order == null)
            {
                return NotFound(id);
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }           
            var joined = await _orderService.PopulateTestItemModel();

            var model = new OrderEditModel
            {
                TestItems = joined.ToArray(),
                Order = order
            };

            return View(model);
        }

        public IActionResult ListUsersOrders(string id)
        {
            var orders = _dbContext.Orders.Where(a => a.CreatorId == id && a.Status != OrderStatusCodes.Created).ToArray();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody]OrderSaveModel model)
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
                var orderToUpdate = await _dbContext.Orders.SingleAsync(a => a.Id == model.OrderId.Value);

                await _orderService.PopulateOrder(model, orderToUpdate);
                _orderService.PopulateOrderWithLabDetails(model, orderToUpdate);

                idForRedirection = model.OrderId.Value;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return Json(new { success = false, message = "Order Id not found." });
            }


            return Json(new { success = true, id = idForRedirection });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(int id, bool confirm)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("OpenOrders");
            }

            order.Status = OrderStatusCodes.Received;

            await _orderService.SendOrderToAnlab(order);

            await _orderMessageService.EnqueueReceivedMessage(order);

            await _dbContext.SaveChangesAsync();

            Message = "Order marked as received";
            return RedirectToAction("OpenOrders");

        }

        public async Task<IActionResult> UpdateFromCompletedTests(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFromCompletedTests(int id, bool confirm)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }

            if (order.Status != OrderStatusCodes.Received)
            {
                ErrorMessage = "You can only Complete a Received order";
                //return RedirectToAction("OpenOrders");
            }

            order.Status = OrderStatusCodes.Complete;

            var result = await _orderService.OverwiteOrderWithTestsCompleted(order); //TODO: Just testing
            if (result.WasError)
            {
                ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                return RedirectToAction("UpdateFromCompletedTests");
            }

            var orderDetails = order.GetOrderDetails();

            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total);

            order.SaveDetails(orderDetails);

            await _dbContext.SaveChangesAsync();

            Message = "Order marked as Complete";
            return RedirectToAction("OpenOrders");

        }

    }
}