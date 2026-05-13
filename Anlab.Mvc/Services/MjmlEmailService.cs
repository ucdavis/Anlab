using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using AnlabMvc.Models.Email;

namespace AnlabMvc.Services
{
    public interface IMjmlEmailService
    {
        Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default);

        Task EnqueueAsync<TModel>(
            string sendTo,
            string subject,
            string templateName,
            TModel model,
            Order order = null,
            User user = null,
            CancellationToken cancellationToken = default);

        Task EnqueueSampleCardEmailAsync(string sendTo, CancellationToken cancellationToken = default);
    }

    public class MjmlEmailService : IMjmlEmailService
    {
        public const string SampleCardTemplateName = "Emails/SampleCard_mjml";

        private readonly IMjmlEmailRenderer _renderer;
        private readonly IMailService _mailService;

        public MjmlEmailService(IMjmlEmailRenderer renderer, IMailService mailService)
        {
            _renderer = renderer;
            _mailService = mailService;
        }

        public Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default)
        {
            return _renderer.RenderAsync(templateName, model, cancellationToken);
        }

        public async Task EnqueueAsync<TModel>(
            string sendTo,
            string subject,
            string templateName,
            TModel model,
            Order order = null,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sendTo))
            {
                throw new ArgumentException("At least one recipient is required.", nameof(sendTo));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject is required.", nameof(subject));
            }

            var body = await RenderAsync(templateName, model, cancellationToken);

            var message = new MailMessage
            {
                SendTo = sendTo,
                Subject = subject,
                Body = body,
                Order = order,
                User = user
            };

            _mailService.EnqueueMessage(message);
        }

        public Task EnqueueSampleCardEmailAsync(string sendTo, CancellationToken cancellationToken = default)
        {
            var model = new SampleCardEmailModel
            {
                PreviewText = "Example MJML email from Anlab.",
                Header = "MJML email example",
                Message = "This sample shows the card-based MJML email layout available to application services.",
                CardTitle = "Work request summary",
                CardSummary = "Use this template as a starting point for future HTML email messages.",
                ButtonText = "View Anlab",
                ButtonUrl = "https://anlab.ucdavis.edu",
                Items = new List<SampleCardEmailItem>
                {
                    new SampleCardEmailItem
                    {
                        Label = "Request",
                        Value = "WR-000123"
                    },
                    new SampleCardEmailItem
                    {
                        Label = "Status",
                        Value = "Received"
                    },
                    new SampleCardEmailItem
                    {
                        Label = "Client",
                        Value = "UC Davis"
                    }
                }
            };

            return EnqueueAsync(sendTo, "Anlab MJML email example", SampleCardTemplateName, model, cancellationToken: cancellationToken);
        }
    }
}
