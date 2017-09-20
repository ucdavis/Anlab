using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class FinancialSettings
    {
        public string SlothApiUrl { get; set; }
        public string AnlabAccount { get; set; }
        public string CreditObjectCode { get; set; }
        public string DebitObjectCode { get; set; }
        public string SlothApiKey { get; set; }
    }
}
