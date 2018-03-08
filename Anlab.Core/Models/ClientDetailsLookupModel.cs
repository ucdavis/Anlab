using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class ClientDetailsLookupModel
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string DefaultAccount { get; set; }
        public string CopyEmail { get; set; }
        public string SubEmail { get; set; }
        public string CopyPhone { get; set; }
        public string SubPhone { get; set; }

        public string Department { get; set; }
    }
}
