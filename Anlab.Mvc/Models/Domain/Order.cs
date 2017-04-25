using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Domain
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public class Order
    {
        public Order()
        {
            Created = DateTime.UtcNow;
        }

        [Required]
        public int Id { get; set; }

        [Range(0.1, double.MaxValue)]
        public decimal Estimate { get; set; }

        public DateTime Created { get; set; }

        // FK reference to user
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

    }
}
