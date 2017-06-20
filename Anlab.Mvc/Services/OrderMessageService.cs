using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Services;

namespace AnlabMvc.Services
{
    public interface IOrderMessageService
    {
        Task EnqueueCreatedMessage(Order order);
    }

    public class OrderMessageService : IOrderMessageService
    {
        private readonly ViewRenderService _viewRenderService;
        private readonly IMailService _mailService;

        public OrderMessageService(ViewRenderService viewRenderService, IMailService mailService)
        {
            _viewRenderService = viewRenderService;
            _mailService = mailService;
        }
        public async Task EnqueueCreatedMessage(Order order)
        {
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_OrderCreated", order);

            var message = new MailMessage
            {
                Subject = "Work Request Confirmation",
                Body = body,
                SendTo = order.Creator.Email
            };

            _mailService.EnqueueMessage(message);
        }
    }
}
