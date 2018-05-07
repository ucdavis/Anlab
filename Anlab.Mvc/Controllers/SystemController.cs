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
#endif
            return RedirectToAction("LogoutDirect", "Account");
        }

    }
}
