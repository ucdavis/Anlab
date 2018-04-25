using Anlab.Core.Models;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Controllers
{
    public class PricesController : Controller
    {
        private readonly IOrderService _orderService;

        public PricesController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        public async Task<IActionResult> Index()
        {
            var joined = await _orderService.PopulateTestItemModel(false);
            TestItemModel[] publicTests = joined.Where(a => a.Public).ToArray(); //Don't really need this as the service with false only returns public.
            return View(publicTests);
        }
    }
}
