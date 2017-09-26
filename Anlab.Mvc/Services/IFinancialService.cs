using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnlabMvc.Services
{
    public interface IFinancialService
    {
        Task<string> GetAccountInfo(string chart, string account);
        Task<string> GetSubAccountInfo(string chart, string account, string subaccount);
    }

    public class FinancialService: IFinancialService
    {
        public async Task<string> GetAccountInfo(string chart, string account)
        {
            using (var client = new HttpClient())
            {
                var url = "https://kfs.ucdavis.edu/kfs-prd/remoting/rest/fau/account/" + chart + "/" + account + "/name";
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
                var url = "https://kfs.ucdavis.edu/kfs-prd/remoting/rest/fau/subaccount/" + chart + "/" + account + "/" + subaccount + "/name";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }
        }
    }
}
