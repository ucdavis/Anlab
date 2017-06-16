using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class SystemController : ApplicationController
    {
        private readonly IDbInitializationService _dbInitializationService;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;

        public SystemController(IDbInitializationService dbInitializationService, ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbInitializationService = dbInitializationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Emulate(string id)
        {
            var user = await _userManager.FindByNameAsync(id);

            if (user == null) return NotFound();

            await _signInManager.SignOutAsync(); // sign out current user

            await _signInManager.SignInAsync(user, false); // sign in new user

            Message = $"Signed in as {id}";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ResetDb()
        {
            await _dbInitializationService.RecreateAndInitialize();

            return RedirectToAction("LogOff", "Account");
        }

    }
}