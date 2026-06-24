namespace AnlabMvc.Models.Email.Billing
{
    public class BillingInformationEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
    }
}
