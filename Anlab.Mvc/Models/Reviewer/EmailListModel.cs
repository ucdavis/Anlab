using Anlab.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AnlabMvc.Models.Reviewer
{
    public class EmailListModel
    {

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Start { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? End { get; set; }
        [Display(Name = "Email Type")]
        public string EmailType { get; set; }
        [Display(Name = "Order Action")]
        public string OrderAction { get; set; }

        public List<string> EmailAddresses { get; set; } = new List<string>();

        public static Expression<Func<MailMessage, string>> Projection(bool PiOnly = true)
        {
            return x =>  PiOnly ? x.Order.Creator.Email : x.Order.AdditionalEmails;
        }
    }
}
