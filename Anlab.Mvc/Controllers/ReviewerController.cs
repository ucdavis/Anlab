using System;
using System.Collections.Generic;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Extensions;
using Anlab.Core.Domain;
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
            if (model.fStart == null && model.fEnd == null && model.cStart == null && model.cEnd == null)
            {
                Message = "Please select a date range.";
                model.Orders = new List<Order>();
                return View(model);
            }

            var orders = _context.Orders.Where(a => a.DateFinalized != null).AsQueryable();
            if (model.fStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.fStart.Value.Date.FromPacificTime());
            }

            if (model.fEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.fEnd.Value.Date.FromPacificTime());
            }

            if (model.cStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.cStart.Value.Date.FromPacificTime());
            }

            if (model.cEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.cEnd.Value.Date.FromPacificTime());
            }

            model.Orders = await orders.ToListAsync();

            return View(model);
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

            await GetHistories(id, model);

            return View(model);
        }

        private async Task GetHistories(int id, OrderReviewModel model)
        {
            model.History = await _context.History.Where(a => a.OrderId == id).Select(s =>
                new History
                {
                    Action = s.Action,
                    ActionDateTime = s.ActionDateTime,
                    Id = s.Id,
                    Status = s.Status,
                    ActorId = s.ActorId,
                    ActorName = s.ActorName,
                    Notes = s.Notes
                }).OrderBy(o => o.ActionDateTime).ToListAsync(); //Basically filtering out jsonDetails
        }
    }
}
