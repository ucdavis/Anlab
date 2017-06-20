using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
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

        public LabController(ApplicationDbContext dbContext, IOrderService orderService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
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


            var model = new OrderEditModel
            {
                TestItems = _dbContext.TestItems.AsNoTracking().ToArray(),
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

                if (orderToUpdate.Status == OrderStatusCodes.Confirmed)
                {
                    orderToUpdate.Status = OrderStatusCodes.Received;
                    await _orderService.SendOrderToAnlab(orderToUpdate);
                }


                idForRedirection = model.OrderId.Value;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return Json(new { success = false, message = "Order Id not found." });
            }


            return Json(new { success = true, id = idForRedirection });
        }
    }
}