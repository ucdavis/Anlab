namespace AnlabMvc.Models.Email.Billing
{
    public class BillingOverrideEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
    }
}
