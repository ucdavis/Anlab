using System;
using System.Linq;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using Anlab.Jobs.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Anlab.Jobs.SendMail
{
    public class Program
    {
        static void Main(string[] args)
        {
            LogHelper.ConfigureLogging();

            var assembyName = typeof(Program).Assembly.GetName();
            Log.Information("Running {job} build {build}", assembyName.Name, assembyName.Version);

            // Use this to get configuration info, environmental comes in from azure
            //var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            //var config = builder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlite("Data Source=anlab.db")
                // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddTransient<IMailService, MailService>();

            var provider = services.BuildServiceProvider();

            var mailService = provider.GetService<IMailService>();
            var dbContext = provider.GetService<ApplicationDbContext>();

            // TODO: remove when using real DB
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            // TOOD: remove, this is how to enqueue a message for testing
            //mailService.EnqueueMessageAsync(new MailMessage
            //{
            //    Body = "Test email",
            //    Subject = "Test time",
            //    SendTo = "srkirkland@ucdavis.edu"
            //}).Wait();

            // Get all messages that we haven't tried to send yet
            var messagesToSend = dbContext.MailMessages.Where(x => x.Sent == null).ToList();

            foreach (var message in messagesToSend)
            {
                try
                {
                    mailService.SendMessage(message);

                    message.Sent = true;
                    message.SentAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // TODO: figure out which exceptions to retry
                    message.Sent = false;
                    message.FailureReason = ex.Message;
                }

                dbContext.Update(message);
                dbContext.SaveChanges();
            }

            Log.Information("Job completed");
        }
    }
}