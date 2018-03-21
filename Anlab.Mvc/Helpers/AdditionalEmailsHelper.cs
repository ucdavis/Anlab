using Anlab.Core.Domain;
using Anlab.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Helpers
{
    public static class AdditionalEmailsHelper
    {
        public static string AddClientInfoEmails (Order order, ClientInfo clientInfo)
        {
            var addEmailList = new List<string> { };
            if (order.AdditionalEmails != null)
                addEmailList = order.AdditionalEmails.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!string.IsNullOrWhiteSpace(clientInfo.Email))
            {
                if (clientInfo.Email != order.Creator.Email && !addEmailList.Contains(clientInfo.Email))
                {
                    addEmailList.Add(clientInfo.Email);
                }
            }

            return string.Join(';', addEmailList);
        }
    }
}
