using System;
using System.Linq;
using Anlab.Core.Data;
using Anlab.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anlab.Jobs.MoneyMovement
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=C:\\GitProjects\\Anlab\\Anlab.Mvc\\bin\\Debug\\netcoreapp1.1\\anlab.db")
                // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );



            var provider = services.BuildServiceProvider();

            var dbContext = provider.GetService<ApplicationDbContext>();


            //Approved payment will only have a value when a CC payment is used.
            var orders = dbContext.Orders.Where(a => a.ApprovedPayment != null && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            
            Console.WriteLine("Hello World!" + orders.Count);
        }
    }
}
