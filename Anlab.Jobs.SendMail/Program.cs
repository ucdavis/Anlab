using System;
using System.IO;
using System.Linq;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using Anlab.Jobs.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Models;
using Microsoft.Extensions.Options;
using Anlab.Jobs.Core.Logging;
using Microsoft.Extensions.Configuration;

namespace Anlab.Jobs.SendMail
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static IServiceProvider Provider { get; set; }
        public static IMailService MailService { get; set; }

        static void Main(string[] args)
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
            Provider = services.BuildServiceProvider();
            MailService = Provider.GetService<IMailService>();

            var dbContext = Provider.GetService<ApplicationDbContext>();

            var emailSettings = new EmailSettings()
            {
                EmailUserName = Configuration.GetSection("Email:EmailUserName").Value,
                EmailPassword = Configuration.GetSection("Email:EmailPassword").Value,
                UseLiveEmails = Configuration.GetSection("Email:UseLiveEmails").Value == "1"
            };

            // Get all messages that we haven't tried to send yet
            var messagesToSend = dbContext.MailMessages.Where(x => x.Sent == null).ToList();
            var counter = 0;

            foreach (var message in messagesToSend)
            {
                try
                {
                    var saveSendTo = message.SendTo;
                    if (!emailSettings.UseLiveEmails)
                    {
                        message.SendTo = "anlab-test@ucdavis.edu";
                    }

                    MailService.SendMessage(message, emailSettings);

                    if (!emailSettings.UseLiveEmails)
                    {
                        message.SendTo = saveSendTo;
                    }

                    message.Sent = true;
                    message.SentAt = DateTime.UtcNow;
                    counter++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
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
