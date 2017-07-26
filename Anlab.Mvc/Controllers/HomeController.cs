//using System.Threading.Tasks;
//using Anlab.Core.Domain;
//using AnlabMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        //private readonly ILabworksService _labworksService;

        //public HomeController(ILabworksService labworksService)
        //{
        //    _labworksService = labworksService;
        //}

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

        //public async Task<IActionResult> Test(string id)
        //{
        //    var rtValue = await _labworksService.GetTestCodesCompletedForOrder(id);

        //    return new JsonResult(rtValue);
        //}
    }
}
