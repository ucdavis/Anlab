using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc
{
    public class AppSettings
    {
        public decimal NonUcRate { get; set; } = 1.5m;
        public string StorageUrlBase { get; set; }
        public string CyberSourceUrl { get; set; }
        public string FinancialLookupUrl { get; set; }
        public string FinancialLookupUser { get; set; }
        public string FinancialLookupSecret { get; set; }
        public string CasBaseUrl { get; set; }
        public string IetWsKey { get; set; }
        /// <summary>
        /// Email for account managers
        /// </summary>
        public string AccountsEmail { get; set; }
        
    }
}
