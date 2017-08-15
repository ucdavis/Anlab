using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Controllers
{
    public class PaymentController : ApplicationController
    {
        private readonly CyberSourceSettings _cyberSourceSettings;

        public PaymentController(IOptions<CyberSourceSettings> cyberSourceSettings)
        {
            _cyberSourceSettings = cyberSourceSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}