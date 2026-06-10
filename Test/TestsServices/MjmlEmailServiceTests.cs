using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using AnlabMvc;
using AnlabMvc.Models.Email.Billing;
using AnlabMvc.Models.Email.Orders;
using AnlabMvc.Models.Email.Samples;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Mjml.Net;
using Shouldly;
using Test.Helpers;
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
            var service = new MjmlEmailService(renderer, mailService, new HttpContextAccessor());

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
            var service = new MjmlEmailService(renderer, mailService, new HttpContextAccessor());
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
        public async Task EnqueueOrderCreatedEmailAsync_UsesOrderCreatedTemplate()
        {
            var renderer = new StubMjmlEmailRenderer();
            var mailService = new StubMailService();
            var service = new MjmlEmailService(renderer, mailService, new HttpContextAccessor());
            var user = new User();
            var order = new Order
            {
                Creator = user
            };

            await service.EnqueueOrderCreatedEmailAsync("client@example.com", order, user);

            renderer.TemplateName.ShouldBe(MjmlEmailService.OrderCreatedTemplateName);
            mailService.Message.ShouldNotBeNull();
            mailService.Message.Subject.ShouldBe("Work Order Confirmation");
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
                html.ShouldContain("https://anlaborders.ucdavis.edu/Images/anlabEmailLogo.jpg");
                html.ShouldContain("mailto:anlab@ucdavis.edu");
                html.ShouldContain("tel:5307520147");
                html.ShouldNotContain("<mjml");
            }
        }

        [Fact]
        public async Task RenderAsync_RendersOrderCreatedTemplateToHtml()
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
                var order = CreateValidEntities.Order(42, populateAllFields: true);
                var orderDetails = order.GetOrderDetails();
                orderDetails.Payment.ClientType = "uc";
                orderDetails.InternalProcessingFee = 12.00m;
                orderDetails.Total = 122.00m;
                orderDetails.SelectedTests = new[]
                {
                    new TestDetails
                    {
                        Id = "PUBLIC",
                        Analysis = "Visible Test",
                        Cost = 40.00m,
                        SetupCost = 2.00m,
                        Total = 42.00m
                    },
                    new TestDetails
                    {
                        Id = "PRIVATE",
                        Analysis = "Hidden Test",
                        Cost = 80.00m,
                        SetupCost = 0.00m,
                        Total = 80.00m
                    },
                    new TestDetails
                    {
                        Id = "REPORTING",
                        Analysis = "Reporting Test",
                        Cost = 25.00m,
                        SetupCost = 0.00m,
                        Total = 25.00m
                    }
                };
                order.SaveDetails(orderDetails);
                order.SaveTestDetails(new[]
                {
                    new TestItemModel
                    {
                        Id = "PUBLIC",
                        Category = "Soil",
                        Public = true
                    },
                    new TestItemModel
                    {
                        Id = "PRIVATE",
                        Category = "Soil",
                        Public = false
                    },
                    new TestItemModel
                    {
                        Id = "REPORTING",
                        Category = "Soil",
                        Public = false,
                        Reporting = true
                    }
                });

                var html = await renderer.RenderAsync(MjmlEmailService.OrderCreatedTemplateName, new OrderCreatedEmailModel
                {
                    Order = order
                });

                html.ShouldContain("Work Order Confirmation");
                html.ShouldContain("Thank you for submitting your work request");
                html.ShouldContain("https://anlab.ucdavis.edu/lab-information/using-the-lab");
                html.ShouldContain("mailto:anlab@ucdavis.edu?subject=Online%20Order%20Number%2042");
                html.ShouldContain("All samples must be numbered sequentially starting at #1.");
                html.ShouldContain("Order Details");
                html.ShouldContain("Test(s)");
                html.ShouldContain("Visible Test");
                html.ShouldNotContain("Hidden Test");
                html.ShouldContain("Processing Fee");
                html.ShouldContain("Grand Total");
                html.ShouldContain("UC Davis Analytical Lab");
                html.ShouldContain("tel:5307520147");
                html.ShouldNotContain("<mjml");
            }
        }

        [Fact]
        public async Task RenderAsync_RendersBillingInformationTemplateToHtml()
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
                var order = CreateValidEntities.Order(2920, populateAllFields: true);
                order.Status = OrderStatusCodes.Finalized;
                order.RequestNum = "22F107";
                var orderDetails = order.GetOrderDetails();
                orderDetails.Payment.ClientType = "uc";
                orderDetails.InternalProcessingFee = 12.00m;
                orderDetails.SelectedTests = new[]
                {
                    new TestDetails
                    {
                        Id = "PUBLIC",
                        Analysis = "Visible Test",
                        Cost = 40.00m,
                        SetupCost = 2.00m,
                        Total = 42.00m
                    },
                    new TestDetails
                    {
                        Id = "PRIVATE",
                        Analysis = "Hidden Test",
                        Cost = 80.00m,
                        SetupCost = 0.00m,
                        Total = 80.00m
                    },
                    new TestDetails
                    {
                        Id = "REPORTING",
                        Analysis = "Reporting Test",
                        Cost = 25.00m,
                        SetupCost = 0.00m,
                        Total = 25.00m
                    }
                };
                order.SaveDetails(orderDetails);
                order.SaveTestDetails(new[]
                {
                    new TestItemModel
                    {
                        Id = "PUBLIC",
                        Category = "Soil",
                        Public = true
                    },
                    new TestItemModel
                    {
                        Id = "PRIVATE",
                        Category = "Soil",
                        Public = false
                    },
                    new TestItemModel
                    {
                        Id = "REPORTING",
                        Category = "Soil",
                        Public = false,
                        Reporting = true
                    }
                });

                var html = await renderer.RenderAsync(MjmlEmailService.BillingInformationTemplateName, new BillingInformationEmailModel
                {
                    Order = order,
                    ButtonText = "Review Details",
                    ButtonUrl = "https://localhost:5001/Reviewer/Details/2920"
                });

                html.ShouldContain("Anlab Work Request Billing");
                html.ShouldContain("Work Request 22F107");
                html.ShouldContain("Order Number 2920");
                html.ShouldContain("A new work request has been placed that requires your attention.");
                html.ShouldContain("Order Details");
                html.ShouldContain("Visible Test");
                html.ShouldContain("Reporting Test");
                html.ShouldNotContain("Hidden Test");
                html.ShouldContain("Review Details");
                html.ShouldContain("https://localhost:5001/Reviewer/Details/2920");
                html.ShouldNotContain("<mjml");
            }
        }

        [Fact]
        public async Task RenderAsync_RendersBillingInformationTemplateAgreementHeaderToHtml()
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
                var order = CreateValidEntities.Order(1458, populateAllFields: true);
                order.Status = OrderStatusCodes.Created;

                var html = await renderer.RenderAsync(MjmlEmailService.BillingInformationTemplateName, new BillingInformationEmailModel
                {
                    Order = order,
                    ButtonText = "Review Details",
                    ButtonUrl = "https://localhost:5001/Reviewer/Details/1458"
                });

                html.ShouldContain("Anlab Agreement Request");
                html.ShouldContain("Order Number 1458");
                html.ShouldContain("A new agreement for a work request has been placed that requires your attention.");
                html.ShouldNotContain("Anlab Work Request Billing");
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
