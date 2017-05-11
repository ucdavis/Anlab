using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anlab.Core.Domain
{
    public class Order
    {
        public Order()
        {
            Created = Updated = DateTime.UtcNow;
        }
        public int Id { get; set; }
        
        [Required]
        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        [StringLength(256)]
        [Required]
        public string Project { get; set; }

        [StringLength(16)]
        public string LabId { get; set; }

        [StringLength(16)]
        public string ClientId { get; set; }
        
        public string AdditionalEmails { get; set; }
        
        public string JsonDetails { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public string Status { get; set; }
    }
}
