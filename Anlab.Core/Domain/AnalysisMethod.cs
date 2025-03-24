using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anlab.Core.Domain
{
    public class AnalysisMethod
    {
        // SOP number
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        [StringLength(256)]
        [Required]
        public string Title { get; set; }

        // soil/plant/water/etc
        [StringLength(256)]
        public string Category { get; set; }

        // Markdown content
        [Required]
        public string Content { get; set; }
    }

    public static class AnalysisCategories
    {
        public static string Soils = "Soils";
        public static string Plant = "Plant";
        public static string Water = "Water";
        public static string Feed = "Feed";
        public static string OliveOil = "Olive Oil";
        public static string ManureAndCompost = "Manure-And-Compost";

        public static readonly string[] All = { Soils, Plant, Water, Feed, OliveOil, ManureAndCompost };
    }
}
