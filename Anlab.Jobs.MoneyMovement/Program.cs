using System;
using System.IO;
using System.Linq;
using Anlab.Core.Data;
using Anlab.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anlab.Jobs.MoneyMovement
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
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
            

            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=C:\\GitProjects\\Anlab\\Anlab.Mvc\\bin\\Debug\\netcoreapp1.1\\anlab.db")
                // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );



            var provider = services.BuildServiceProvider();

            var dbContext = provider.GetService<ApplicationDbContext>();


            //Approved payment will only have a value when a CC payment is used.
            var orders = dbContext.Orders.Where(a => a.ApprovedPayment != null && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();

            var xxx = Configuration.GetSection("MoneyMovement:SlothApiKey").Value;
            var yyy = Configuration.GetSection("MoneyMovement:SlothApiUrl").Value;

            Console.WriteLine("MoneyMovement:");
            Console.Write(xxx);
            Console.Write(yyy);
            


            Console.WriteLine("Hello World!" + orders.Count);
        }
    }
}
