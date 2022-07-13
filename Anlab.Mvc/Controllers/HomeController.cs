using Anlab.Core.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.SystemAlertModels;

namespace AnlabMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        
        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var alerts = await _dbContext.SystemAlerts.Where(a => a.IsActive).OrderByDescending(a => a.Updated).Select(a => new AlertModel{ Content = a.Content, Danger = a.Danger}).ToListAsync();
            return View(alerts);
        }
        public IActionResult SamplingAndPreparation()
        {
            return View();
        }

        public IActionResult TestException()
        {
            throw new Exception("Test exception. If this was a real exception, you would need to run in circles, scream and shout.");
        }

        public IActionResult TestNotFound(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            return NotFound(id);
        }


    }
}
