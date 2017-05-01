using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class SystemController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SystemController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ResetDb()
        {
            await _context.Database.EnsureDeletedAsync();

            await DbInitializer.Initialize(_context, _userManager);

            await _context.Database.EnsureCreatedAsync();
            
            return RedirectToAction("LogOff", "Account");
        }
        
    }
}