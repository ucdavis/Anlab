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
        public async Task<IActionResult> ResetDb()
        {
#if DEBUG
            await _dbInitializationService.RecreateAndInitialize();
#else
            throw new NotImplementedException("WHAT!!! Don't reset DB in Release!");
#endif
            return RedirectToAction("LogoutDirect", "Account");
        }

    }
}
