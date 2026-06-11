using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc;
using AnlabMvc.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Test.TestsServices
{
    public class OrderMessageServiceTests
    {
        [Fact]
        public async Task EnqueueReceivedMessage_WhenBypassedSendsToAnlabAndPassesClientRecipientsForCard()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueWorkRequestReceivedByLabEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object);
            var order = CreateOrder();

            await orderMessageService.EnqueueReceivedMessage(order, bypass: true);

            mjmlEmailService.Verify(a => a.EnqueueWorkRequestReceivedByLabEmailAsync(
                "anlab@example.com",
                order,
                order.Creator,
                true,
                "client@example.com;copy@example.com",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnqueueReceivedMessage_WhenNotBypassedSendsToClientRecipients()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueWorkRequestReceivedByLabEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object);
            var order = CreateOrder();

            await orderMessageService.EnqueueReceivedMessage(order);

            mjmlEmailService.Verify(a => a.EnqueueWorkRequestReceivedByLabEmailAsync(
                "client@example.com;copy@example.com",
                order,
                order.Creator,
                false,
                null,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        private static OrderMessageService CreateService(IMjmlEmailService mjmlEmailService)
        {
            return new OrderMessageService(
                null,
                null,
                Options.Create(new AppSettings()),
                Options.Create(new EmailSettings
                {
                    AnlabAddress = "anlab@example.com"
                }),
                mjmlEmailService);
        }

        private static Order CreateOrder()
        {
            return new Order
            {
                Creator = new User
                {
                    Email = "client@example.com"
                },
                AdditionalEmails = "copy@example.com",
                RequestNum = "22F107"
            };
        }
    }
}
