using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using AnlabMvc;
using AnlabMvc.Models.Email.Samples;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Mjml.Net;
using Shouldly;
using Xunit;

namespace Test.TestsServices
{
    public class MjmlEmailServiceTests
    {
        [Fact]
        public async Task EnqueueAsync_RendersMjmlTemplateAndQueuesMailMessage()
        {
            var renderer = new StubMjmlEmailRenderer();
            var mailService = new StubMailService();
            var service = new MjmlEmailService(renderer, mailService);

            await service.EnqueueAsync(
                "client@example.com",
                "Rendered email",
                MjmlEmailService.SampleCardTemplateName,
                new { Header = "Hello" });

            renderer.TemplateName.ShouldBe(MjmlEmailService.SampleCardTemplateName);
            mailService.Message.ShouldNotBeNull();
            mailService.Message.SendTo.ShouldBe("client@example.com");
            mailService.Message.Subject.ShouldBe("Rendered email");
            mailService.Message.Body.ShouldBe(StubMjmlEmailRenderer.RenderedHtml);
        }

        [Fact]
        public async Task EnqueueSampleCardEmailAsync_UsesSampleTemplate()
        {
            var renderer = new StubMjmlEmailRenderer();
            var mailService = new StubMailService();
            var service = new MjmlEmailService(renderer, mailService);
            var order = new Order();
            var user = new User();

            await service.EnqueueSampleCardEmailAsync("client@example.com", order, user);

            renderer.TemplateName.ShouldBe(MjmlEmailService.SampleCardTemplateName);
            mailService.Message.ShouldNotBeNull();
            mailService.Message.Subject.ShouldBe("Anlab MJML email example");
            mailService.Message.Order.ShouldBe(order);
            mailService.Message.User.ShouldBe(user);
        }

        [Fact]
        public async Task RenderAsync_RendersSampleCardTemplateToHtml()
        {
            var services = new ServiceCollection();
            var webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var diagnosticListener = new DiagnosticListener("MjmlEmailServiceTests");

            services.AddLogging();
            services.AddSingleton<DiagnosticSource>(diagnosticListener);
            services.AddSingleton(diagnosticListener);
            services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment
            {
                ApplicationName = typeof(Startup).Assembly.GetName().Name,
                ContentRootPath = AppContext.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
                EnvironmentName = Environments.Development,
                WebRootPath = webRoot,
                WebRootFileProvider = Directory.Exists(webRoot)
                    ? new PhysicalFileProvider(webRoot)
                    : new NullFileProvider()
            });
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSingleton<MjmlRenderer>();
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(Startup).Assembly);
            services.AddTransient<IMjmlEmailRenderer, MjmlEmailRenderer>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var renderer = serviceProvider.GetRequiredService<IMjmlEmailRenderer>();

                var html = await renderer.RenderAsync(MjmlEmailService.SampleCardTemplateName, new SampleCardEmailModel
                {
                    Header = "Render test",
                    Message = "This email should render.",
                    CardTitle = "Card title",
                    CardSummary = "Card summary",
                    Items = new[]
                    {
                        new SampleCardEmailItem
                        {
                            Label = "Status",
                            Value = "Received"
                        }
                    }
                });

                html.ShouldContain("Render test");
                html.ShouldContain("Card title");
                html.ShouldContain("Status");
                html.ShouldNotContain("<mjml");
            }
        }

        private class StubMjmlEmailRenderer : IMjmlEmailRenderer
        {
            public const string RenderedHtml = "<html><body>Rendered MJML</body></html>";

            public string TemplateName { get; private set; }

            public Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default)
            {
                TemplateName = templateName;
                return Task.FromResult(RenderedHtml);
            }
        }

        private class StubMailService : IMailService
        {
            public MailMessage Message { get; private set; }

            public void EnqueueMessage(MailMessage message)
            {
                Message = message;
            }

            public void SendMessage(MailMessage mailMessage)
            {
            }

            public void SendFailureNotification(int OrderId, string failureReason)
            {
            }

            public bool IsSendDisabled()
            {
                return false;
            }
        }

        private class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string ApplicationName { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public string EnvironmentName { get; set; }
            public string WebRootPath { get; set; }
            public IFileProvider WebRootFileProvider { get; set; }
        }
    }
}
