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

        public FakeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync("test@test.com");

            return Content("Test");
        }
    }
}
