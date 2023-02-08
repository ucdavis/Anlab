using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using System.Collections.Generic;

namespace Anlab.Core.Models.AggieEnterpriseModels
{
    public class AccountValidationModel
    {
        public bool IsValid { get; set; } = true;

        public string FinancialSegmentString { get; set; } 
        public FinancialChartStringType CoaChartType { get; set; }
        public GlSegments GlSegments { get; set; }
        public PpmSegments PpmSegments { get; set; }

        public string AccountManager { get; set; }
        public string AccountManagerEmail { get; set; }
        public string ProjectName { get; set; }

        public string DisplayName { get; set; }


        /// <summary>
        /// Return Segment info.
        /// </summary>
        public List<KeyValuePair<string, string>> Details { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Warnings { get; set; } = new List<KeyValuePair<string, string>>();
        public string Message
        {
            get
            {
                if (Messages.Count <= 0)
                {
                    return string.Empty;
                }

                return string.Join(" ", Messages);
            }
        }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
