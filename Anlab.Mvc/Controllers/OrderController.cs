using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Anlab.Core.Models;
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

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId).ToArrayAsync();

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new OrderEditModel { TestItems = _context.TestItems.AsNoTracking().ToArray() };

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

                await PopulateOrder(model, orderToUpdate);


                idForRedirection = model.OrderId.Value;
                await _context.SaveChangesAsync();
            }
            else
            {
                var order = new Order
                {
                    CreatorId = CurrentUserId,
                };
                await PopulateOrder(model, order);

                _context.Add(order);
                await _context.SaveChangesAsync();
                idForRedirection = order.Id;
            }
            

            return Json(new { success = true, id = idForRedirection });
        }

        private async Task PopulateOrder(OrderSaveModel model, Order orderToUpdate)
        {
            orderToUpdate.Project = model.Project;
            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();
            var testItemIds = orderDetails.SelectedTests.Select(a => a.Id).ToArray();
            var selectedTests = await _context.TestItems.Where(a => testItemIds.Contains(a.Id)).ToListAsync();
            if (string.Equals(orderDetails.Payment.ClientType, "uc", StringComparison.OrdinalIgnoreCase))
            {
                orderDetails.Total = selectedTests.Sum(a => a.InternalCost);
            }
            else
            {
                if (string.Equals(orderDetails.Payment.ClientType, "other", StringComparison.OrdinalIgnoreCase))
                {
                    orderDetails.Total = selectedTests.Sum(a => a.ExternalCost);
                }
                else
                {
                    throw new Exception("What! unknown payment!!!");
                }
            }
            orderDetails.Total = orderDetails.Total * orderDetails.Quantity;
            orderDetails.Total += selectedTests.Sum(a => a.SetupCost);

            orderToUpdate.SaveDetails(orderDetails);
            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);
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