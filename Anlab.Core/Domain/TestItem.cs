using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Anlab.Core.Domain
{
    public class TestItem
    {
        [Key]
        [StringLength(128)]
        [Display(Name = "Code")]
        [RegularExpression(@"([A-Z0-9a-z\-#])+", ErrorMessage = "Codes can only contain alphanumerics, #, and dashes.")]
        public string Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Analysis { get; set; }       


        [Required]
        [StringLength(64)]
        public string Category { get; set; }

        [Required]
        [StringLength(8)]
        public string Group { get; set; }

        public bool Public { get; set; }

        public string Notes { get; set; }
        public string NotesEncoded
        {
            get
            {
                var encoder = HtmlEncoder.Default;
                return encoder.Encode(Notes);
            }
        }
    }

    public static class TestCategories
    {
        public static string Soil = "Soil";
        public static string Plant = "Plant";
        public static string Water = "Water";
        public static string Other = "Other";
    }
}
