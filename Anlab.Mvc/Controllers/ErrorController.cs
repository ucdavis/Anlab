using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class ErrorController : Controller
    {
        private const string errKey = "ErrorMessage";
        [HttpGet]
        public IActionResult Index(int id)
        {
            return View(id);
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult LoginError(int id)
        {
            switch (id)
            {
                case 101:
                    TempData[errKey] = "Do not use Google to log in to a UC Davis account";
                    break;
                case 102:
                    TempData[errKey] = "Error from external provider. Please try again and if the problem persists please contact us.";
                    break;
                case 103:
                    TempData[errKey] = "Error from CAS provider. Please try again and if the problem persists please contact us.";
                    break;
                default:
                    TempData[errKey] = "An error occurred trying to sign in. Please try again and if the problem persists please contact us.";
                    break;
            }
            
            return View();
        }
    }
}
