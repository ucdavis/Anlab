using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            var result = Task.Run(() => ProcessCreditCardOrders()).Result; //Wasn't able to debug this unless it returned a result...
            if (!result)
            {
                Console.WriteLine("No CC Orders to process");
            }
            Console.WriteLine("Done Credit Cards");

           
            Console.WriteLine("Done");
        }

        public static async Task<bool> ProcessCreditCardOrders()
        {
            var dbContext = Provider.GetService<ApplicationDbContext>();
            var token = Configuration.GetSection("Financial:SlothApiKey").Value;
            var url = Configuration.GetSection("Financial:SlothApiUrl").Value;
            var orders = dbContext.Orders.Include(i => i.ApprovedPayment).Where(a =>
                a.ApprovedPayment != null && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                return false;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{url}Transactions/processor/");
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

                Console.WriteLine($"Processing {orders.Count} orders");
                var updatedCount = 0;
                foreach (var order in orders)
                {
                    var response = await client.GetAsync(order.ApprovedPayment.Transaction_Id);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        continue;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var slothResponse = JsonConvert.DeserializeObject<SlothResponseModel>(content);

                        updatedCount++;
                        order.KfsTrackingNumber = slothResponse.KfsTrackingNumber;
                        order.SlothTransactionId = slothResponse.Id.ToString();
                        order.Status = OrderStatusCodes.Complete;
                    }
                }

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Updated {updatedCount} orders");
            }

            return true;
        }


    }
}
