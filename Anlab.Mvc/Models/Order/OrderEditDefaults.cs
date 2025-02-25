using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public class OrderEditDefaults
    {
        public string DefaultEmail { get; set; }

        public string DefaultCopyEmail { get; set; }

        public string DefaultSubEmail { get; set; }

        public string DefaultClientId { get; set; }

        public string DefaultAccount { get; set; }

        public string DefaultClientIdName { get; set; }

        public string DefaultCompanyName { get; set; }

        public string DefaultAcName { get; set; }

        public string DefaultAcAddr { get; set; }

        public string DefaultAcPhone { get; set; }

        public string DefaultAcEmail { get; set; }

        public bool DefaultPlacingOrder { get; set; } = true;
    }
}
