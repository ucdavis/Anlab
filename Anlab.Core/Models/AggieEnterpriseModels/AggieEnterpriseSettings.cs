using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Models.AggieEnterpriseModels
{
    public class AggieEnterpriseSettings
    {
        public string GraphQlUrl { get; set; }
        public string Token { get; set; }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string TokenEndpoint { get; set; }
        public string ScopeApp { get; set; }
        public string ScopeEnv { get; set; }

        public bool UseCoA { get; set; }

        public string AnlabCoa { get; set; }
        public string NaturalAccount { get; set; }
    }
}
