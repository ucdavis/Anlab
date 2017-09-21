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
            //var temp = rawAccount.Split('-');
            //var tempSub = temp[1].Split('/');

            //Chart = temp[0];
            //if (tempSub.Length > 1)
            //{
            //    Account = tempSub[0];
            //    SubAccount = tempSub[1];
            //}
            //else
            //{
            //    Account = temp[1];
            //}
            var temp = Regex.Split(rawAccount, @"(\w)-(\w{7})\/?(\w{5})?");
            Chart = temp[1];
            Account = temp[2];
            SubAccount = temp[3];

            //string pattern = @"(\w)-(\w{7})\/?(\w{5})?";
            //Regex rgx = new Regex(pattern);
            //foreach (Match match in rgx.Matches(rawAccount))
            //{
            //    var test1 = match.Value;
            //    var zzz = 1;
            //}
            ////var temp2 = Regex.Matches(rawAccount, @"(\w)-(\w{7})\/?(\w{5})?");
            ////foreach (var VARIABLE in temp2)
            ////{
            ////    var test = VARIABLE;
            ////}
            ////var aaa = 1;
        }
        public string Chart { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
    }
}
