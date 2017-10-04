namespace Anlab.Core.Domain
{
    public class AnalysisMethod
    {
        // SOP number
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        // soil/plant/water/etc
        public string Category { get; set; }
        
        // Markdown content
        public string Content { get; set; }
    }
}