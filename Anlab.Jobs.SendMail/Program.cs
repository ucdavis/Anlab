using System;
using System.Linq;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anlab.Jobs.SendMail
{
    public class Program
    {
        static void Main(string[] args)
        {
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

            using (var txn = dbContext.Database.BeginTransaction())
            {
                mailService.EnqueueMessageAsync(new MailMessage
                {
                    Body = "Test email",
                    Subject = "Test time",
                    SendTo = "srkirkland@ucdavis.edu"
                }).Wait();

                Console.WriteLine(dbContext.MailMessages.Count());

                txn.Commit();
            }

            Console.WriteLine("Hello World!");

            Console.ReadKey();
        }
    }
}