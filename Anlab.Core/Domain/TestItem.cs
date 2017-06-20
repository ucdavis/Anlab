using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anlab.Core.Domain
{
    public class TestItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(7)]
        public string FeeSchedule { get; set; }

        [Required]
        [StringLength(512)]
        public string Analysis { get; set; }
        
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public decimal InternalCost { get; set; }

        public decimal ExternalCost { get; set; }

        public decimal SetupCost { get; set; }

        [Required]
        [StringLength(64)]
        public string Category { get; set; }

        [Required]
        [StringLength(8)]
        public string Group { get; set; }

        [Range(0, int.MaxValue)]
        public int Multiplier { get; set; }

        public bool Multiplies { get; set; }

        public bool ChargeSet { get; set; }

        public bool Public { get; set; }

        public string GroupType { get; set; }

        public string Notes { get; set; }
    }

    public static class TestCategories
    {
        public static string Soil = "Soil";
        public static string Plant = "Plant";
        public static string Water = "Water";
        public static string Other = "Other";
    }
}
