using System.ComponentModel.DataAnnotations.Schema;

namespace Anlab.Core.Domain
{
    public class AnalysisMethod
    {
        // SOP number
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        // soil/plant/water/etc
        public string Category { get; set; }
        
        // Markdown content
        public string Content { get; set; }
    }
}