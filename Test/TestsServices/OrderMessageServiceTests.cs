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
        public async Task EnqueueCreatedMessage_WhenMjmlFlagEnabledUsesMjmlEmail()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueOrderCreatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
            var order = CreateOrder();

            await orderMessageService.EnqueueCreatedMessage(order);

            mjmlEmailService.Verify(a => a.EnqueueOrderCreatedEmailAsync(
                "client@example.com;copy@example.com",
                order,
                order.Creator,
                It.IsAny<CancellationToken>()), Times.Once);
        }

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
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
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
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
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

        [Fact]
        public async Task EnqueuePartialResultsMessage_WhenMjmlFlagEnabledSendsToAnlabAndPassesClientRecipientsForCard()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueWorkRequestPartialResultsEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
            var order = CreateOrder();

            await orderMessageService.EnqueuePartialResultsMessage(order);

            mjmlEmailService.Verify(a => a.EnqueueWorkRequestPartialResultsEmailAsync(
                "anlab@example.com",
                order,
                order.Creator,
                true,
                "client@example.com;copy@example.com",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnqueueFinalizedMessage_WhenBypassedSendsToAnlabAndPassesClientRecipientsForCard()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueWorkRequestFinalizedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
            var order = CreateOrder();

            await orderMessageService.EnqueueFinalizedMessage(order, bypass: true);

            mjmlEmailService.Verify(a => a.EnqueueWorkRequestFinalizedEmailAsync(
                "anlab@example.com",
                order,
                order.Creator,
                true,
                "client@example.com;copy@example.com",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnqueueFinalizedMessage_WhenNotBypassedSendsToClientRecipients()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueWorkRequestFinalizedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
            var order = CreateOrder();

            await orderMessageService.EnqueueFinalizedMessage(order);

            mjmlEmailService.Verify(a => a.EnqueueWorkRequestFinalizedEmailAsync(
                "client@example.com;copy@example.com",
                order,
                order.Creator,
                false,
                null,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnqueueBillingMessage_WhenMjmlFlagEnabledUsesMjmlEmail()
        {
            var mjmlEmailService = new Mock<IMjmlEmailService>();
            mjmlEmailService
                .Setup(a => a.EnqueueBillingInformationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Order>(),
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var orderMessageService = CreateService(mjmlEmailService.Object, useMjmlEmails: true);
            var order = CreateOrder();

            await orderMessageService.EnqueueBillingMessage(order, "Billing subject");

            mjmlEmailService.Verify(a => a.EnqueueBillingInformationEmailAsync(
                "accounts@example.com",
                "Billing subject",
                order,
                order.Creator,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        private static OrderMessageService CreateService(IMjmlEmailService mjmlEmailService, bool useMjmlEmails)
        {
            return new OrderMessageService(
                null,
                null,
                Options.Create(new AppSettings
                {
                    UseMjmlEmails = useMjmlEmails,
                    AccountsEmail = "accounts@example.com"
                }),
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
