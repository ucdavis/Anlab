using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
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
        public IActionResult Index()
        {
            var model = _context.Orders.ToList();

            return View(model);
        }


        public async Task<IActionResult> MyOrders()
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

            var order = new Order
            {
                CreatorId = CurrentUserId,
                Project = "Project name",
                JsonDetails = JsonConvert.SerializeObject(model)
            };
            // save model

            _context.Add(order);

            await _context.SaveChangesAsync();

            return Json(new { success = true, id = order.Id });
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
            model.OrderDetails = JsonConvert.DeserializeObject<OrderDetails>(order.JsonDetails);
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
                return RedirectToAction("MyOrders");
            }

            order.Status = "Confirmed";
            await _context.SaveChangesAsync();

            Message = "Order confirmed";
            return RedirectToAction("MyOrders");

        }
    }

    public class OrderSaveModel {
        public int Quantity { get;set;}
        public string SampleType {get;set;}

        public string AdditionalInfo { get; set; }

        public TestItem[] SelectedTests {get;set;} 
        public decimal Total {get;set;}

        public Payment Payment { get; set; }
    }

    public class Payment
    {
        public string ClientType { get; set; }
        public string Account { get; set; }
    }

   
}