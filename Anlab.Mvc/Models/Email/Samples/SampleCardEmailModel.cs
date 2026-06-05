using System.Collections.Generic;
using AnlabMvc.Models.Email;

namespace AnlabMvc.Models.Email.Samples
{
    public class SampleCardEmailModel : MjmlEmailTemplateModelBase
    {
        public string Header { get; set; }
        public string Message { get; set; }
        public string CardTitle { get; set; }
        public string CardSummary { get; set; }
        public IReadOnlyList<SampleCardEmailItem> Items { get; set; } = new List<SampleCardEmailItem>();
    }

    public class SampleCardEmailItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
