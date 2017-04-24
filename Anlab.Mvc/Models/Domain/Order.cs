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
        [Required]
        public int Id { get; set; }

        [Range(0.1, double.MaxValue)]
        public decimal Estimate { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

    }
}
