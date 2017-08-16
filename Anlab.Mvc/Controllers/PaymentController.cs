using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using Anlab.Core.Data;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Controllers
{
    public class PaymentController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataSigningService _dataSigningService;
        private readonly CyberSourceSettings _cyberSourceSettings;

        public PaymentController(ApplicationDbContext context, IDataSigningService dataSigningService, IOptions<CyberSourceSettings> cyberSourceSettings)
        {
            _context = context;
            _dataSigningService = dataSigningService;
            _cyberSourceSettings = cyberSourceSettings.Value;
        }

        public ActionResult Pay(int id)
        {

            return View();
        }


    }
}