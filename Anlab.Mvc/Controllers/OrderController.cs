using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class OrderController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        public OrderController(ApplicationDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId).ToArrayAsync();

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new OrderEditModel { TestItems = _context.TestItems.AsNoTracking().ToArray() };

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

            if (order.Status != null)
            {
                ErrorMessage = "You can't edit an order that has been confirmed.";
                return RedirectToAction("Index");
            }


            var model = new OrderEditModel {
                TestItems = _context.TestItems.AsNoTracking().ToArray(),
                Order = order
            };

            return View(model); 
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
                var orderToUpdate = await _context.Orders.SingleAsync(a => a.Id == model.OrderId.Value);
                if (orderToUpdate.CreatorId != CurrentUserId)
                {
                    return Json(new {success = false, message = "This is not your order."});
                }
                if(orderToUpdate.Status != null)
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

            if (order.Status == null)
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

            if (order.Status != null)
            {
                ErrorMessage = "Already confirmed";
                return RedirectToAction("Index");
            }

            order.Status = "Confirmed";
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

            if (order.Status != null)
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