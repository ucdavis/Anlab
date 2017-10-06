using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class ClientIdLookupModel
    {
        public string ClientId { get; set; }
        public string EMail { get; set; }
        public string PhoneNumber { get; set; }
        public string CopyEmail { get; set; }
        public string CopyPhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
