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
    }
}
