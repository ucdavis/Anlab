using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Jobs.MoneyMovement
{
    public class AccountModel
    {
        public AccountModel(string rawAccount)
        {
            var temp = rawAccount.Split('-');
            var tempSub = temp[1].Split('/');

            Chart = temp[0];
            if (tempSub.Length > 1)
            {
                Account = tempSub[0];
                SubAccount = tempSub[1];
            }
            else
            {
                Account = temp[1];
            }
        }
        public string Chart { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
    }
}
