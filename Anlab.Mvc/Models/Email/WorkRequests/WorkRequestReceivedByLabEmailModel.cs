namespace AnlabMvc.Models.Email.WorkRequests
{
    public class WorkRequestReceivedByLabEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
    }
}
