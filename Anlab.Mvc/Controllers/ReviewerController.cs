using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anlab.Core.Data;
using Microsoft.EntityFrameworkCore;
using Anlab.Core.Models;
using Microsoft.AspNetCore.Authorization;
using AnlabMvc.Models.Roles;

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
            var model = await _context.Orders.Where(a => a.Status == OrderStatusCodes.Finalized).ToArrayAsync();

            return View(model);
        }
    }
}
