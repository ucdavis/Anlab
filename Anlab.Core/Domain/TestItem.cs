using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Encodings.Web;

namespace Anlab.Core.Domain
{
    public class TestItem
    {
        [Key]
        [StringLength(128)]
        [Display(Name = "Code")]
        [RegularExpression(@"([A-Z0-9a-z\-#_%()])+", ErrorMessage = "Codes can only contain alphanumerics, #, _, %, (, ), and dashes.")]
        public string Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Analysis { get; set; }


        [Required]
        [StringLength(64)]
        public string Category { get; set; }

        [NotMapped]
        public string[] Categories
        {
            get => Category != null ? Category.Split('|') : new string[0];
            set => Category = string.Join("|", value);
        }

        [StringLength(64)]
        public string TestGroup { get; set; } = string.Empty;

        public string[] TestGroups
        {
            get => TestGroup != null ? TestGroup.Split('|') : new string[0];
            set => TestGroup = string.Join("|", value);
        }

        [Required]
        [StringLength(512)]
        public string Group { get; set; }

        public bool Public { get; set; }
        public bool Reporting { get; set; } = false;

        public bool DryMatter { get; set; } = false;

        public string AdditionalInfoPrompt { get; set; }

        public string Notes { get; set; }
        public string NotesEncoded
        {
            get
            {
                if (Notes == null)
                {
                    return null;
                }
                var encoder = HtmlEncoder.Default;
                return encoder.Encode(Notes);
            }
        }

        public int RequestOrder { get; set; }
        public int LabOrder { get; set; }
    }

    public static class TestCategories
    {
        public static string Soil = "Soil";
        public static string Plant = "Plant";
        public static string Water = "Water";
        public static string Miscellaneous = "Miscellaneous";

        public static readonly string[] All = { Soil, Plant, Water, Miscellaneous };
    }

    /// <summary>
    /// Will be used to filter what tests a click can choose.
    /// </summary>
    public static class TestGroups
    {
        public static string AnLab = "AnLab";
        public static string SIF = "SIF";
        public static string ICPMS = "ICPMS";
        public static string CDFA = "CDFA";
        public static readonly string[] All = { AnLab, SIF, ICPMS, CDFA };
    }
}
