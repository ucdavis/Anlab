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
