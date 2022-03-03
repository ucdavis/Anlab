using System;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin)]
    public class SystemController : ApplicationController
    {
        private readonly IDbInitializationService _dbInitializationService;

        public SystemController(IDbInitializationService dbInitializationService)
        {
            _dbInitializationService = dbInitializationService;
        }

#if DEBUG
        public async Task<IActionResult> ResetDb()
        {
            await _dbInitializationService.RecreateAndInitialize();
            return RedirectToAction("LogoutDirect", "Account");
        }
#else
        public Task<IActionResult> ResetDb()
        {
            throw new NotImplementedException("WHAT!!! Don't reset DB in Release!");
        }
#endif

    }
}
