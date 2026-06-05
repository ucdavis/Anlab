namespace AnlabMvc.Models.Email.Orders
{
    public class OrderCreatedEmailModel : MjmlEmailTemplateModelBase
    {
        public Anlab.Core.Domain.Order Order { get; set; }       
    }
}
