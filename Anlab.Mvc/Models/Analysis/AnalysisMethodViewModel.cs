using System.Collections.Generic;
using Anlab.Core.Domain;

namespace AnlabMvc.Models.Analysis
{
    public class AnalysisMethodViewModel
    {
        public AnalysisMethod AnalysisMethod { get; set; }
        
        public string HtmlContent { get; set; }
        
        public List<AnalysisMethod> AnalysesInCategory { get; set; }
    }
}