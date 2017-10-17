using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class SampleTypeQuestions
    {
        public bool SoilImported { get; set; }
        public string PlantReportingBasis { get; set; }
        public bool WaterFiltered { get; set; }
        public bool WaterPreservativeAdded { get; set; }
        public string WaterPreservativeInfo { get; set; }
        public bool WaterReportedInMgL { get; set; }
    }
}
