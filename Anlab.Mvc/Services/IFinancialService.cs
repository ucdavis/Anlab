using Anlab.Jobs.MoneyMovement;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace AnlabMvc.Services
{
    public interface IFinancialService
    {
        Task<string> GetAccountName(string account);
    }

    public class FinancialService : IFinancialService
    {
        private readonly AppSettings _appSettings;

        public FinancialService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<string> GetAccountName(string account)
        {
            //https://kfs.ucdavis.edu/kfs-prd/api-docs/ //Documentation Maybe this is changing to https://kfs.ucdavis.edu/fin/api-docs/ ???
            var accountModel = new AccountModel(account);
            string url;
            string validationUrl;
            //https://financials.api.adminit.ucdavis.edu/fau/subaccount/{chart}/{account}/{subaccount}/isvalid
            if (!string.IsNullOrWhiteSpace(accountModel.SubAccount))
            {
                validationUrl =
                    $"{_appSettings.FinancialLookupUrl}/subaccount/{accountModel.Chart}/{accountModel.Account}/{accountModel.SubAccount}/isvalid";
                url =
                    $"{_appSettings.FinancialLookupUrl}/subaccount/{accountModel.Chart}/{accountModel.Account}/{accountModel.SubAccount}/name";
            }
            else
            {
                validationUrl = $"{_appSettings.FinancialLookupUrl}/account/{accountModel.Chart}/{accountModel.Account}/isvalid";
                url = $"{_appSettings.FinancialLookupUrl}/account/{accountModel.Chart}/{accountModel.Account}/name";
            }

            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(validationUrl);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();
                if (!JsonConvert.DeserializeObject<bool>(validationContents))
                {
                    Log.Information($"Account not valid {account}");
                    throw new Exception("Invalid Account");
                }


                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();


                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }

        }
    }

}
