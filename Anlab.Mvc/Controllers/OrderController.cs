using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            return Json(new { success = true });
        }
    }

    public class OrderSaveModel {
        public int quantity { get;set;}
        public string SampleType {get;set;}
    }
}