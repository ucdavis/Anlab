using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnlabMvc.Models.MailMessageModels
{
    public class EditMailMessageModel
    {
        public EditMailMessageModel(){}
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public string SendTo { get; set; }
        [Required]
        public string Subject { get; set; }
        public string FailureReason { get; set; }

        public bool Resend { get; set; } = false;
        [Display(Name = "Mark As Sent")]
        public bool Unsend { get; set; } = false;

        public bool? Sent { get; set; }
        public int FailureCount { get; set; }

        public EditMailMessageModel(MailMessage v)
        {
            Id = v.Id;
            SendTo = v.SendTo;
            Subject = v.Subject;
            FailureReason = v.FailureReason;
            OrderId = v.Order.Id;
            Sent = v.Sent;
            FailureCount = v.FailureCount;
        }
    }
}
