using System;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Reviewer;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.Reports)]
    public class ReviewerController : ApplicationController
    {
        private readonly ApplicationDbContext _context;

        public ReviewerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lastActions = DateTime.UtcNow.AddDays(-60);
            var model = await _context.Orders.Where(a => a.Status == OrderStatusCodes.Finalized || (a.Status == OrderStatusCodes.Complete && a.Updated >= lastActions)).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Totals(ReviewerTotalModel model)
        {
            if (fStart == null && fEnd == null && cStart == null && cEnd == null)
            {
                Message = "Please select a date range.";
                return View(null);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

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
    }
}
