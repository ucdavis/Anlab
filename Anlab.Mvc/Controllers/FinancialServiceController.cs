using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Anlab.Jobs.MoneyMovement;
using System.Net;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class FinancialServiceController : ApplicationController
    {
        private readonly IFinancialService _financialService;
        
        public FinancialServiceController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("financial/info")]
        public async Task<string> GetAccountInfo(string account)
        {           
            var result = await _financialService.GetAccountName(account);

            return result;
        }
    }
}
