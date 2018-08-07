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
            if (value.Equals("GRNDONLY", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsGroupTest(this string value)
        {
            if (value.StartsWith("G-", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
