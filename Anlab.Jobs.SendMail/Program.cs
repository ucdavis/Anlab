using Anlab.Core.Data;
using Anlab.Core.Models;
using Anlab.Core.Services;
using Anlab.Jobs.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;


namespace Anlab.Jobs.SendMail
{
    public class Program
    {
        private const int MaxEmails = 30;
        public static IConfigurationRoot Configuration { get; set; }
        public static IServiceProvider Provider { get; set; }
        public static IMailService MailService { get; set; }
        
        static void Main()
        {
            // Use this to get configuration info, environmental comes in from azure
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.Equals(environmentName, "development", StringComparison.OrdinalIgnoreCase))
            {
                builder.AddUserSecrets<Program>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            LogHelper.ConfigureLogging(Configuration);

            var assembyName = typeof(Program).Assembly.GetName();
            Log.Information("Running {job} build {build}", assembyName.Name, assembyName.Version);

            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddTransient<IMailService, MailService>();
            
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            

            Provider = services.BuildServiceProvider();
            MailService = Provider.GetService<IMailService>();

            var dbContext = Provider.GetService<ApplicationDbContext>();

            if (MailService.IsSendDisabled())
            {
                Log.Information("Email Send Disabled");
                return;
            }
            

            // Get all messages that have not been sent, including failed ones for now
            var skippingCount = dbContext.MailMessages.Count(a => (a.Sent == null || a.Sent == false) && a.FailureCount > 2);
            if (skippingCount > 0)
            {
                Log.Information($"Emails skipped because of failure count: {skippingCount}");
            }
            var messagesToSend = dbContext.MailMessages.Where(x => (x.Sent == null || x.Sent == false) && x.FailureCount <= 2).Take(MaxEmails).ToList();
            Log.Information($"Emails to Send: {messagesToSend.Count}");
            var counter = 0;

            foreach (var message in messagesToSend)
            {
                try
                {
                    if (message.Sent == false && message.FailureCount > 2)
                    {
                        continue;
                    }

                    MailService.SendMessage(message);

                    message.Sent = true;
                    message.SentAt = DateTime.UtcNow;
                    counter++;
                }
                catch (SmtpFailedRecipientException failedRecipientEx)
                {
                    Log.Error(failedRecipientEx.Message);
                    message.FailureCount = 5; //Say it failed 5 times so it doesn't try to send again. Otherwise need to change the db.
                    var orderId = dbContext.MailMessages.Where(a => a.Id == message.Id).Select(s => s.Order.Id).Single();

                    MailService.SendFailureNotification(orderId, failedRecipientEx.Message);
                    message.Sent = false;
                    message.FailureReason = failedRecipientEx.Message;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    message.FailureCount++;
                    if (message.FailureCount > 2)
                    {
                        var messageWithOrder = dbContext.MailMessages.Include(i => i.Order)
                            .SingleOrDefault(a => a.Id == message.Id);
                        int orderId = 0;
                        if (messageWithOrder != null && messageWithOrder.Order != null)
                        {
                            orderId = messageWithOrder.Order.Id;
                        }

                        MailService.SendFailureNotification(orderId, ex.Message);
                    }

                    // TODO: figure out which exceptions to retry
                    message.Sent = false;
                    message.FailureReason = ex.Message;
                }

                dbContext.Update(message);
                dbContext.SaveChanges();
            }

            Log.Information($"Emails Sent: {counter}");

            Log.Information("Job completed");
        }
    }
}
