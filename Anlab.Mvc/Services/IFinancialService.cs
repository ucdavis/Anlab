using Anlab.Jobs.MoneyMovement;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Anlab.Core.Extensions;
using Newtonsoft.Json;

namespace AnlabMvc.Services
{
    public interface IFinancialService
    {
        Task<string> GetAccountName(string account);
    }

    public class FinancialService: IFinancialService
    {
        private readonly AppSettings _appSettings;
        public FinancialService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        
        public async Task<string> GetAccountName(string account)
        {

            var accountModel = new AccountModel(account);
            string url;
            if (!String.IsNullOrWhiteSpace(accountModel.SubAccount))
            {
                url = String.Format("{0}/subaccount/{1}/{2}/{3}", _appSettings.FinancialLookupUrl,
                    accountModel.Chart, accountModel.Account, accountModel.SubAccount);
            }
            else
            {
                url = String.Format("{0}/account/{1}/{2}", _appSettings.FinancialLookupUrl, accountModel.Chart,
                    accountModel.Account);
            }
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();


                var contents = await response.Content.ReadAsStringAsync();
                var kfsDetails = JsonConvert.DeserializeObject<KfsLookup>(contents);
                if (kfsDetails.Closed || kfsDetails.AccountExpirationDate != null && kfsDetails.AccountExpirationDate.Value <= DateTime.UtcNow.ToPacificTime())
                {
                    throw new Exception("Closed or expired");
                }
                return JsonConvert.SerializeObject(kfsDetails.AccountName);
            }

        }
    }

    public class KfsLookup
    {
        public string AccountName { get; set; }
        public DateTime? AccountExpirationDate { get; set; }

        public bool Closed { get; set; }
    }
}
