using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;

namespace AnlabMvc.Controllers
{
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

        public IActionResult Create()
        {
            var model = new OrderEditModel {TestItems = _context.TestItems.AsNoTracking().ToArray()};

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]OrderSaveModel model)
        {
            // TODO: do validation
            if (model.OrderId.HasValue)
            {
                throw new Exception("oops");
            }

            var order = new Order
            {
                CreatorId = CurrentUserId,
                Project = model.Project,
                JsonDetails = JsonConvert.SerializeObject(model)
            };
            // save model

            _context.Add(order);

            await _context.SaveChangesAsync();

            return Json(new { success = true, id = order.Id });
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
            var testItemIds = model.OrderDetails.SelectedTests.Select(a => a.Id).ToArray();

            //TODO: Should this be done in the create?
            var selectedTests = await _context.TestItems.Where(a => testItemIds.Contains(a.Id)).ToListAsync();
            if(string.Equals(model.OrderDetails.Payment.ClientType, "uc", StringComparison.OrdinalIgnoreCase))
            {
                model.OrderDetails.Total = selectedTests.Sum(a => a.InternalCost);
            }
            else
            {
                if (string.Equals(model.OrderDetails.Payment.ClientType, "other", StringComparison.OrdinalIgnoreCase))
                {
                    model.OrderDetails.Total = selectedTests.Sum(a => a.ExternalCost);
                }
                else
                {
                    throw new Exception("What! unknown payment!!!");
                }
            }

            model.OrderDetails.Total = model.OrderDetails.Total * model.OrderDetails.Quantity;
            model.OrderDetails.Total += selectedTests.Sum(a => a.SetupCost);

            order.SaveDetails(model.OrderDetails);
            order.AdditionalEmails = model.OrderDetails.AdditionalEmails.Any() ? string.Join(";", model.OrderDetails.AdditionalEmails) : null;

            await _context.SaveChangesAsync();


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
    }

    public class OrderSaveModel {
        public int? OrderId { get; set; }

        public int Quantity { get;set;}
        public string SampleType {get;set;}

        public string AdditionalInfo { get; set; }

        public TestItem[] SelectedTests {get;set;} 
        public decimal Total {get;set;}

        public string Project { get; set; }

        public Payment Payment { get; set; }

        public IList<string> AdditionalEmails { get; set; }
    }



   
}