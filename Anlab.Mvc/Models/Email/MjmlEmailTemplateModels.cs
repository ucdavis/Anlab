using System.Collections.Generic;

namespace AnlabMvc.Models.Email
{
    public abstract class MjmlEmailTemplateModelBase
    {
        public string AppName { get; set; } = "UC Davis Analytical Laboratory";
        public string PreviewText { get; set; }
        public string ButtonText { get; set; }
        public string ButtonUrl { get; set; }
        public string LayoutWidth { get; set; }
    }

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

    public class MjmlEmailButtonModel
    {
        public MjmlEmailButtonModel(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; }
        public string Url { get; }
    }
}
