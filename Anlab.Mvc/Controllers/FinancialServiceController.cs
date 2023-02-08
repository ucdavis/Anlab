using AggieEnterpriseApi.Validation;
using Anlab.Core.Models.AggieEnterpriseModels;
using Anlab.Core.Services;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class FinancialServiceController : ApplicationController
    {
        private readonly IFinancialService _financialService;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;
        private readonly AggieEnterpriseSettings _aeSettings;

        public FinancialServiceController(IFinancialService financialService, IAggieEnterpriseService aggieEnterpriseService, IOptions<AggieEnterpriseSettings> aeSettings)
        {
            _financialService = financialService;
            _aggieEnterpriseService = aggieEnterpriseService;
            _aeSettings = aeSettings.Value;
        }

        [HttpGet("financial/info")]
        public async Task<string> GetAccountInfo(string account)
        {
            if(_aeSettings.UseCoA)
            {
                var accountValidation = await _aggieEnterpriseService.IsAccountValid(account);
                if (accountValidation.IsValid)
                {
                    var name = accountValidation.CoaChartType == FinancialChartStringType.Ppm ? $"PPM: {accountValidation.ProjectName} ({accountValidation.AccountManager})" : "GL Account";
                        //AccountManagerName = accountValidation.AccountManager,
                        //AccountManagerEmail = accountValidation.AccountManagerEmail, 
                        //Number = accountValidation.FinancialSegmentString
                    return name;
                }
                throw new Exception("Invalid Account"); // Yuck, need to return a model with errors
            }
            
            var result = await _financialService.GetAccountName(account);

            return result;
        }
    }
}
