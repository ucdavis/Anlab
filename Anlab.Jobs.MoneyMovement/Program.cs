using Anlab.Core.Data;
using Anlab.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using Anlab.Core.Models;
using Microsoft.Extensions.Options;

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
            var financialSettings = new FinancialSettings
            {
                SlothApiKey = Configuration.GetSection("Financial:SlothApiKey").Value,
                SlothApiUrl = Configuration.GetSection("Financial:SlothApiUrl").Value
            };
            
            var result = Task.Run(() => SlothService.ProcessCreditCards(financialSettings)).Result; //Wasn't able to debug this unless it returned a result...
            if (!result)
            {
                Console.WriteLine("No CC Orders to process");
            }
            Console.WriteLine("Done Credit Cards");

            result = Task.Run(() => SlothService.MoneyHasMoved(financialSettings)).Result;
            if (!result)
            {
                Console.WriteLine("No UCD Accounts were completed");
            }

           
            Console.WriteLine("Done");
        }
    }
}
