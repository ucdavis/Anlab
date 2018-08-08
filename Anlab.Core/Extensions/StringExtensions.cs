using System;
using System.Globalization;

namespace Anlab.Core.Extensions
{
    public static class StringExtensions
    {
        public static string SafeToUpper(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToUpper(CultureInfo.InvariantCulture);
        }

        public static bool ClearOutSetupPrice(this string value)
        {
            //If we have other special cases, add them here
            return value.Equals("GRNDONLY", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsGroupTest(this string value)
        {
            return value.StartsWith("G-", StringComparison.OrdinalIgnoreCase);
        }
    }
}
