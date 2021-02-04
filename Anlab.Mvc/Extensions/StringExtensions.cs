using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PhoneNumbers;

namespace AnlabMvc.Extensions
{
    public static class StringExtensions
    {
        //const string emailRegex = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        const string emailRegex = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$"; //https://www.regular-expressions.info/email.html

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

        public static bool IsEmailValid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return Regex.IsMatch(value, emailRegex, RegexOptions.IgnoreCase);
        }

        public static string FormatPhone(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                var phoneNumber = phoneNumberUtil.ParseAndKeepRawInput(value, "US");
                var phone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.NATIONAL);
                if (!phoneNumberUtil.IsValidNumberForRegion(phoneNumber, "US"))
                {
                    if (phoneNumberUtil.IsValidNumber(phoneNumber))
                    {
                        return phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                    }

                    return phoneNumber.RawInput;
                }

                return phone;
            }
            catch (Exception)
            {
                return value;
            }

        }
    }
}
