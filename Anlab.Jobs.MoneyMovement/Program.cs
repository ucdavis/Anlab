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
                    options.UseSqlite("Data Source=anlab.db")
                // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );



            var provider = services.BuildServiceProvider();

            var dbContext = provider.GetService<ApplicationDbContext>();



            var orders = dbContext.Orders.Where(a => a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            
            Console.WriteLine("Hello World!");
        }
    }
}
