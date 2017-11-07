using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using Microsoft.AspNetCore.Mvc;
using AnlabMvc.Services;

namespace AnlabMvc.Controllers
{
    public class PricesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        public PricesController(ApplicationDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }
        
        public async Task<IActionResult> Index()
        {
            var joined = await _orderService.PopulateTestItemModel(false);
            TestItemModel[] publicTests = joined.Where(a => a.Public).ToArray();
            return View(publicTests);
        }
    }
}
