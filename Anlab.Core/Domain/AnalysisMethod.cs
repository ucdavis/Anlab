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
}