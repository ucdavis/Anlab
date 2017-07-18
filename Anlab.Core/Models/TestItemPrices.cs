using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anlab.Core.Models
{
    public class TestItemPrices
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public decimal Cost { get; set; }

        public decimal SetupCost { get; set; }


    }
}
