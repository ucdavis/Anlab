using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
