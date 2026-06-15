using System;

namespace AnlabMvc.Models.Email
{
    public abstract class MjmlEmailTemplateModelBase
    {
        public string AppName { get; set; } = "UC Davis Analytical Laboratory";
        public string PreviewText { get; set; }
        public string ButtonText { get; set; }
        public string ButtonUrl { get; set; }
        public string LayoutWidth { get; set; }
        public string FooterImageUrl { get; set; } = "https://anlaborders.ucdavis.edu/Images/anlabEmailLogo.jpg";
        public bool BypassClientEmail { get; set; }
        public string BypassRecipientList { get; set; }
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

    public class ResultsDownloadLinkEmailModel
    {
        public ResultsDownloadLinkEmailModel(string downloadUrl)
        {
            DownloadUrl = IsHttpOrHttpsAbsoluteUrl(downloadUrl) ? downloadUrl : null;
        }

        public string DownloadUrl { get; }

        public static bool IsHttpOrHttpsAbsoluteUrl(string downloadUrl)
        {
            return !string.IsNullOrWhiteSpace(downloadUrl)
                   && Uri.TryCreate(downloadUrl, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class OrderTestDetailsTableEmailModel
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public bool ShowBillingTotals { get; set; }
    }
}
