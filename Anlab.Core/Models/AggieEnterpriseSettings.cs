using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Models
{
    public class AggieEnterpriseSettings
    {
        public string GraphQlUrl { get; set; }
        public string Token { get; set; }

        public bool UseCoA { get; set; }
    }
}
