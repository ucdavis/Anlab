using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anlab.Core.Models
{
    public class Payment
    {
        [Required(ErrorMessage = "You must select the payment method.")]
        public string ClientType { get; set; }
        public string Account { get; set; }

        public string AccountName { get; set; }

        public bool IsInternalClient => string.Equals(ClientType, "uc", StringComparison.OrdinalIgnoreCase);
    }
}
