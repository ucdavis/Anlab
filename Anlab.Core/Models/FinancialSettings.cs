using Serilog;
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

        public string SafeSlothApiUrl
        {
            get
            {
                Log.Information("SlothApiUrl: {SlothApiUrl}", SlothApiUrl);
                if (SlothApiUrl.EndsWith("v1/", StringComparison.OrdinalIgnoreCase) || SlothApiUrl.EndsWith("v2/", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Error("Sloth SlothApiUrl should not end with version");
                    //Replace the end of the string
                    return SlothApiUrl.Substring(0, SlothApiUrl.Length - 3);
                }
                return SlothApiUrl;
            }
        }
    }
}
