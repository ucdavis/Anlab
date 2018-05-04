using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin)]
    public class SystemController : ApplicationController
    {
        private readonly IDbInitializationService _dbInitializationService;
        private readonly SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public SystemController(IDbInitializationService dbInitializationService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbInitializationService = dbInitializationService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Emulate(string id)
        {
            var saveUser = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(id);

            if (user == null) return NotFound();

            await _signInManager.SignOutAsync(); // sign out current user

            await _signInManager.SignInAsync(user, false); // sign in new user

            Message = $"Signed in as {id}";
            Log.Information($"{saveUser} Emulation of {id}");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ResetDb()
        {
#if DEBUG
            await _dbInitializationService.RecreateAndInitialize();
#endif
            return RedirectToAction("LogoutDirect", "Account");
        }

    }
}
