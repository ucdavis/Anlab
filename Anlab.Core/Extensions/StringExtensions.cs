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
            return string.Equals(value, "GRNDONLY", StringComparison.OrdinalIgnoreCase); //Deals with null, which would only happen in my tests
        }

        public static bool IsGroupTest(this string value)
        {
            return value.StartsWith("G-", StringComparison.OrdinalIgnoreCase);
        }

        public static string SpecialTruncation(this string value, int additionalLength, int maxLength)
        {
            if ((value.Length + additionalLength) <= maxLength)
            {
                return value;
            }

            var max = (value.Length + additionalLength + 3) - maxLength;
            return $"{value.Substring(0, (value.Length - max))}...";
        }
    }
}
