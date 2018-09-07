using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnlabMvc.Models.MailMessageModels
{
    public class EditMailMessageModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public string FailureReason { get; set; }

        public bool Resend { get; set; } = false;

        public EditMailMessageModel(MailMessage v)
        {
            Id = v.Id;
            SendTo = v.SendTo;
            Subject = v.Subject;
            FailureReason = v.FailureReason;
            OrderId = v.Order.Id;
        }
    }
}
