using System;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILabworksService _labworksService;

        public HomeController(ApplicationDbContext context, ILabworksService labworksService)
        {
            _context = context;
            _labworksService = labworksService;
        }
        public IActionResult Index()
        {
            return View();
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

        public async Task<IActionResult> Ping()
        {
            var localDbCheck = _context.TestItems.FirstOrDefault(a => a.Public);
            if (localDbCheck == null)
            {
                throw new Exception("Unable to get local db record");
            }

            var code = await _labworksService.TestDbConnection();

            return Content("pong");
        }
    }
}
