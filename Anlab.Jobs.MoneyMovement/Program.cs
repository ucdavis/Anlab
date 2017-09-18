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
            Provider = services.BuildServiceProvider();

            Console.WriteLine("About to start Credit Cards");
            var result = Task.Run(() => ProcessCreditCardOrders()).Result; //Wasn't able to debug this unless it returned a result...
            if (!result)
            {
                Console.WriteLine("No CC Orders to process");
            }
            Console.WriteLine("Done Credit Cards");
            Console.WriteLine("About to start UC Davis Account Transfer");
            var result2 = Task.Run(() => ProcessAccountTransfers()).Result; //Wasn't able to debug this unless it returned a result...
            if (!result2)
            {
                Console.WriteLine("No Orders to process");
            }
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
                        order.Status = OrderStatusCodes.Complete;
                    }
                }

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Updated {updatedCount} orders");
            }

            return true;
        }


        public static async Task<bool> ProcessAccountTransfers()
        {
            var dbContext = Provider.GetService<ApplicationDbContext>();
            var token = Configuration.GetSection("Financial:SlothApiKey").Value;
            var url = Configuration.GetSection("Financial:SlothApiUrl").Value;

            var creditAccount = new AccountModel(Configuration.GetSection("Financial:AnlabAccount").Value);
            
            var objectCode = Configuration.GetSection("Financial:ObjectCode").Value;

            var orders = dbContext.Orders.Where(a => a.ApprovedPayment == null && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                return false;
            }
            //TODO: Need a process to filter out all orders that are not UC with chart 3/L
            
            var xxx = new TransactionViewModel();
            xxx.MerchantTrackingNumber = "12";
            xxx.ProcessorTrackingNumber = "12";
            xxx.Transfers.Add(new TransferViewModel{Account = "12345", Amount = 100m, Chart = "3", Description = "Test", Direction = "Debit", ObjectCode = objectCode });
            xxx.Transfers.Add(new TransferViewModel { Account = creditAccount.Account, Amount = 100m, Chart = creditAccount.Chart, Description = "Test", Direction = "Credit", ObjectCode = objectCode, SubAccount = creditAccount.SubAccount});

            var yyy = JsonConvert.SerializeObject(xxx);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

                var test = new StringContent(JsonConvert.SerializeObject(xxx), System.Text.Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("Transactions", test);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No tFound");
                }
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("No Content");
                }


                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var slothResponse = JsonConvert.DeserializeObject<SlothResponseModel>(content);

                }
                //else
                //{
                    var content1 = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(JsonConvert.DeserializeObject(content1));
                //}

                var ttt = response;
            }

            return true;
        }

    }
}
