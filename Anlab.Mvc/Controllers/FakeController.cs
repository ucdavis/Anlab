using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class FakeController : ApplicationController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public FakeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync("test@test.com");

            var xxx = await _userManager.IsInRoleAsync(user, "Test");

            await _signInManager.SignOutAsync(); // sign out current user

            await _signInManager.SignInAsync(user, false); // sign in new user

            return Content("Test");
        }
    }
}
