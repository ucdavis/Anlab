using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Extensions
{
    public static class StringExtensions
    {
        public static string PaymentMethodDescription(this string value)
        {
            if (string.Equals(value, "uc", StringComparison.OrdinalIgnoreCase))
            {
                return "UC Account";
            }

            if (string.Equals(value, "creditcard", StringComparison.OrdinalIgnoreCase))
            {
                return "Credit Card";
            }

            return "Other";
        }
    }
}
