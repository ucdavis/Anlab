using System;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using AnlabMvc.Models.Email.Billing;
using AnlabMvc.Models.Email.Orders;
using AnlabMvc.Models.Email.Payments;
using AnlabMvc.Models.Email.WorkRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

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

        Task EnqueueOrderCreatedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default);

        Task EnqueueWorkRequestReceivedByLabEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default);

        Task EnqueueWorkRequestPartialResultsEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default);

        Task EnqueueWorkRequestFinalizedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default);

        Task EnqueueBillingInformationEmailAsync(
            string sendTo,
            string subject,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default);

        Task EnqueuePaymentReceivedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default);

        Task EnqueueBillingOverrideEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default);

        Task EnqueueDisposalWarningEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default);
    }

    public class MjmlEmailService : IMjmlEmailService
    {
        public const string SampleCardTemplateName = "Emails/Samples/SampleCard_mjml";
        public const string OrderCreatedTemplateName = "Emails/Orders/OrderCreated_mjml";
        public const string WorkRequestReceivedByLabTemplateName = "Emails/WorkRequests/WorkRequestReceivedByLab_mjml";
        public const string WorkRequestPartialResultsTemplateName = "Emails/WorkRequests/WorkRequestPartialResults_mjml";
        public const string WorkRequestFinalizedTemplateName = "Emails/WorkRequests/WorkRequestFinalized_mjml";
        public const string BillingInformationTemplateName = "Emails/Billing/BillingInformation_mjml";
        public const string PaymentReceivedTemplateName = "Emails/Payments/PaymentReceived_mjml";
        public const string BillingOverrideTemplateName = "Emails/Billing/BillingOverride_mjml";
        public const string DisposalWarningTemplateName = "Emails/WorkRequests/DisposalWarning_mjml";

        private readonly IMjmlEmailRenderer _renderer;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;

        public MjmlEmailService(IMjmlEmailRenderer renderer, IMailService mailService, IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings = null)
        {
            _renderer = renderer;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings?.Value ?? new AppSettings();
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

        public Task EnqueueOrderCreatedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            var model = new OrderCreatedEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Work Order Confirmation",
                //ButtonText = "Your Orders", //This might cause confusion since people can be emailed when they didn't create the order and will not have access to it.
                //ButtonUrl = $"https://anlaborders.ucdavis.edu/Order/"
            };

            return EnqueueAsync(sendTo, "Work Order Confirmation", OrderCreatedTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueueWorkRequestReceivedByLabEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default)
        {
            var model = new WorkRequestReceivedByLabEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Work Request Received By Lab",
                BypassClientEmail = bypassClientEmail,
                BypassRecipientList = bypassRecipientList ?? sendTo
            };

            var subject = $"Work Request Confirmation - {order?.RequestNum}";
            if (bypassClientEmail)
            {
                subject = $"{subject} -- Bypass Client";               
            }

            return EnqueueAsync(sendTo, subject, WorkRequestReceivedByLabTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueueWorkRequestPartialResultsEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default)
        {
            var includeResultsDownloadLink = ShouldIncludePartialResultsDownloadLink(order);
            var model = new WorkRequestPartialResultsEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Work Request Partial Results",
                BypassClientEmail = bypassClientEmail,
                BypassRecipientList = bypassRecipientList ?? sendTo,
                ShowResultsDownloadLink = includeResultsDownloadLink,
                ResultsDownloadUrl = includeResultsDownloadLink
                    ? BuildResultsDownloadUrl(order)
                    : null
            };

            var subject = $"Work Request Partial Results - {order?.RequestNum}";
            if (bypassClientEmail)
            {
                subject = $"{subject} -- Bypass Client";
            }

            return EnqueueAsync(sendTo, subject, WorkRequestPartialResultsTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        private bool ShouldIncludePartialResultsDownloadLink(Order order)
        {
            return _appSettings.IncludePartialResultsDownloadLink
                   && !string.IsNullOrWhiteSpace(order?.ResultsFileIdentifier);
        }

        public Task EnqueueBillingInformationEmailAsync(
            string sendTo,
            string subject,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            var model = new BillingInformationEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Anlab Agreement Request",
                ButtonText = "Review Details",
                ButtonUrl = BuildReviewerDetailsUrl(order)
            };

            return EnqueueAsync(sendTo, subject, BillingInformationTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueuePaymentReceivedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            var model = new PaymentReceivedEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Payment Complete",
                ButtonText = "View Your Results",
                ButtonUrl = BuildResultsLinkUrl(order)
            };

            var subject = $"Work Request Payment Complete  - {order?.RequestNum}";

            return EnqueueAsync(sendTo, subject, PaymentReceivedTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueueBillingOverrideEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            var model = new BillingOverrideEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Anlab Order -- Admin Override"
            };

            return EnqueueAsync(sendTo, "Anlab Order -- Admin Override", BillingOverrideTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueueWorkRequestFinalizedEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            bool bypassClientEmail = false,
            string bypassRecipientList = null,
            CancellationToken cancellationToken = default)
        {
            var orderDetails = order?.GetOrderDetails();
            var isInternalClient = orderDetails?.Payment?.IsInternalClient == true;
            var buttonText = isInternalClient ? "Get Your Results" : "Get Your Results and Pay";
            var model = new WorkRequestFinalizedEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Work Request Finalized - Payment Pending",
                ButtonText = buttonText,
                ButtonUrl = BuildResultsLinkUrl(order),
                BypassClientEmail = bypassClientEmail,
                BypassRecipientList = bypassRecipientList ?? sendTo
            };

            var subject = $"Work Request Finalized - Payment Pending - {order?.RequestNum}";
            if (bypassClientEmail)
            {
                subject = $"{subject} -- Bypass Client";
            }

            return EnqueueAsync(sendTo, subject, WorkRequestFinalizedTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        public Task EnqueueDisposalWarningEmailAsync(
            string sendTo,
            Order order,
            User user = null,
            CancellationToken cancellationToken = default)
        {
            var model = new DisposalWarningEmailModel
            {
                LayoutWidth = "800px",
                Order = order,
                PreviewText = "Work Request Disposal Warning",
                ButtonText = "View Details of Order",
                ButtonUrl = BuildResultsLinkUrl(order)
            };

            var subject = $"Work Request Disposal Warning - {order?.RequestNum}";

            return EnqueueAsync(sendTo, subject, DisposalWarningTemplateName, model, order, user ?? order?.Creator, cancellationToken);
        }

        private string BuildReviewerDetailsUrl(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("A current HTTP request is required to build the reviewer details email URL.");
            }

            return UriHelper.BuildAbsolute(
                request.Scheme,
                request.Host,
                request.PathBase,
                $"/Reviewer/Details/{order.Id}");
        }

        private string BuildResultsLinkUrl(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("A current HTTP request is required to build the results email URL.");
            }

            return UriHelper.BuildAbsolute(
                request.Scheme,
                request.Host,
                request.PathBase,
                $"/Results/Link/{order.ShareIdentifier}");
        }

        private string BuildResultsDownloadUrl(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("A current HTTP request is required to build the results download email URL.");
            }

            return UriHelper.BuildAbsolute(
                request.Scheme,
                request.Host,
                request.PathBase,
                $"/Results/Download/{order.ShareIdentifier}");
        }
    }
}
