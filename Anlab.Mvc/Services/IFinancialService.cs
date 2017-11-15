using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services
{
    public interface IFinancialService
    {
        Task<string> GetAccountInfo(string chart, string account);
        Task<string> GetSubAccountInfo(string chart, string account, string subaccount);
    }

    public class FinancialService: IFinancialService
    {
        private readonly AppSettings _appSettings;
        public FinancialService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> GetAccountInfo(string chart, string account)
        {
            using (var client = new HttpClient())
            {
                var url = String.Format("{0}/account/{1}/{2}/name", _appSettings.FinancialLookupUrl, chart, account);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }
        }
        public async Task<string> GetSubAccountInfo(string chart, string account, string subaccount)
        {
            using (var client = new HttpClient())
            {
                var url = String.Format("{0}/subaccount/{1}/{2}/{3}/name", _appSettings.FinancialLookupUrl, chart, account, subaccount);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }
        }
    }
}
