using AggieEnterpriseApi.Validation;
using Anlab.Core.Models.AggieEnterpriseModels;
using Anlab.Core.Services;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
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
        public async Task<AccountValidationModel> GetAccountInfo(string account)
        {
            var rtValue = new AccountValidationModel();
            if (_aeSettings.UseCoA)
            {
                rtValue = await _aggieEnterpriseService.IsAccountValid(account);
                if (rtValue.IsValid)
                {
                    rtValue.DisplayName = rtValue.CoaChartType == FinancialChartStringType.Ppm ? $"PPM: {rtValue.ProjectName} ({rtValue.AccountManager})" : "GL Account";
                        //AccountManagerName = accountValidation.AccountManager,
                        //AccountManagerEmail = accountValidation.AccountManagerEmail, 
                        //Number = accountValidation.FinancialSegmentString
                }

                return rtValue;
            }
            try
            { 
                var result = await _financialService.GetAccountName(account);
                rtValue.DisplayName = result;
                rtValue.IsValid = true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to validate/find KFS account");
                rtValue.IsValid = false;
                rtValue.Messages.Add("Unable to validate/find KFS account");
            }
            return rtValue;
        }       
    }
}
