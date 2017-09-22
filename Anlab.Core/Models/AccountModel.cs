using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Anlab.Jobs.MoneyMovement
{
    public class AccountModel
    {
        public AccountModel(string rawAccount)
        {
            var regx = Regex.Match(rawAccount, @"(\w)-(\w{7})\/?(\w{5})?");
            Chart = regx.Groups[1].Value;
            Account = regx.Groups[2].Value;
            SubAccount = regx.Groups[3].Value;
        }
        public string Chart { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
    }
}
