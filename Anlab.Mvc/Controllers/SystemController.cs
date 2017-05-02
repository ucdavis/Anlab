using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
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

        public SystemController(IDbInitializationService dbInitializationService)
        {
            _dbInitializationService = dbInitializationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ResetDb()
        {
            await _dbInitializationService.RecreateAndInitialize();
            
            return RedirectToAction("LogOff", "Account");
        }
        
    }
}