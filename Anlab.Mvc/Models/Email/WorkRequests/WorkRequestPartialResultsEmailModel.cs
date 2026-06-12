namespace AnlabMvc.Models.Email.WorkRequests
{
    public class WorkRequestPartialResultsEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public bool ShowResultsDownloadLink { get; set; }
        public string ResultsDownloadUrl { get; set; }
    }
}
