using Anlab.Jobs.MoneyMovement;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            //https://kfs.ucdavis.edu/kfs-prd/api-docs/ //Documentation
            var accountModel = new AccountModel(account);
            string url;
            string validationUrl;
            if (!String.IsNullOrWhiteSpace(accountModel.SubAccount))
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
                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "User", "Secret"));
                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                //Need this for MIV (PrePurchasing) otherwise it throws an exception if it is empty.
                //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");


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
