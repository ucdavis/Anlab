namespace AnlabMvc.Models.Email.Payments
{
    public class PaymentReceivedEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }
    }
}
