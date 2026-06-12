namespace AnlabMvc.Models.Email.WorkRequests
{
    public class WorkRequestFinalizedEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
    }
}
