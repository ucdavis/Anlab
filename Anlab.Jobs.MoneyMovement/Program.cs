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
using Anlab.Jobs.Core.Logging;
using Serilog;
using Anlab.Core.Models.AggieEnterpriseModels;

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
            
            LogHelper.ConfigureLogging(Configuration);

            var assembyName = typeof(Program).Assembly.GetName();
            Log.Information("Running {job} build {build}", assembyName.Name, assembyName.Version);


            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.Configure<FinancialSettings>(Configuration.GetSection("Financial"));
            services.Configure<AggieEnterpriseSettings>(Configuration.GetSection("AggieEnterprise"));

            
            services.AddTransient<IAggieEnterpriseService, AggieEnterpriseService>();

            services.AddTransient<ISlothService, SlothService>();           
            Provider = services.BuildServiceProvider();
            
            SlothService = Provider.GetService<ISlothService>();

            Log.Information("Job Starting");
          

            SlothService.ProcessCreditCards().GetAwaiter().GetResult();
            SlothService.MoneyHasMoved().GetAwaiter().GetResult();

            Log.Information("Job Done");
        }
    }
}
