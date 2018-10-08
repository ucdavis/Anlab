using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anlab.Core.Domain
{
    public class History
    {
        public History()
        {
            ActionDateTime = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public DateTime ActionDateTime { get; set; }

        public string Action { get; set; }
        public string ActorId { get; set; }
        public string ActorName { get; set; }
        public string Status { get; set; }

        public string Notes { get; set; }

        public string JsonDetails { get; set; } //I'm going to make a copy of the new details for dev use.
    }   
}
