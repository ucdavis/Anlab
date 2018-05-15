using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Models;

namespace AnlabMvc.Models.Order
{
    public class OverwriteOrderResult
    {
        public OverwriteOrderResult()
        {
            SelectedTests = new List<TestDetails>();
            MissingCodes = new List<string>();
        }
        public IList<TestDetails> SelectedTests { get; set; }
        public IList<String> MissingCodes { get; set; }
        public string ErrorMessage { get; set; }
        
        public string ClientId { get; set; }
        public int Quantity { get; set; }
        
        public decimal RushMultiplier { get; set; }
        public string LabworksSampleDisposition { get; set; }

        public bool WasError => MissingCodes.Count > 0 || ErrorMessage != null;
    }
}
