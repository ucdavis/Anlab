using System;
using System.ComponentModel.DataAnnotations;
using Anlab.Core.Models;

namespace Anlab.Core.Domain
{
    public class MailMessage {
        public MailMessage()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Required]
        public string SendTo { get; set; }

        public bool? Sent { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public string FailureReason { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; }

        public User User { get; set; }
    }
}
