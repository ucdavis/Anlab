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

        public static string MaxLength(this string value, int maxLength, bool ellipsis = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length > maxLength)
            {
                if (ellipsis)
                {
                    return $"{value.Substring(0, maxLength - 3)}...";
                }

                return value.Substring(0, maxLength);
            }

            return value;
        }
    }
}
