using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Configuration
{
    public class AzureOptions
    {
        public string TenantName { get; set; }
        public string TentantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
