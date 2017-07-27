using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SamplingAndPreparation()
        {
            ViewData["Message"] = "Si .";

            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
