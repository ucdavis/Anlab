using Anlab.Core.Data;
using Anlab.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Anlab.Jobs.MoneyMovement
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static IServiceProvider Provider { get; set; }
        
        public static ISlothService SlothService { get; set; }

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
                    options.UseSqlite("Data Source=..\\Anlab.Mvc\\anlab.db")
                // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddTransient<ISlothService, SlothService>();
            Provider = services.BuildServiceProvider();
            
            SlothService = Provider.GetService<ISlothService>();           

            Console.WriteLine("About to start Credit Cards");
            //var result = Task.Run(() => ProcessCreditCardOrders()).Result; //Wasn't able to debug this unless it returned a result...
            var result = Task.Run(() => SlothService.ProcessCreditCards(Configuration)).Result; //Wasn't able to debug this unless it returned a result...
            if (!result)
            {
                Console.WriteLine("No CC Orders to process");
            }
            Console.WriteLine("Done Credit Cards");

           
            Console.WriteLine("Done");
        }
    }
}
