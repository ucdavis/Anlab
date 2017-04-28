using Anlab.Core.Domain;
using AnlabMvc.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Test.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category", "DatabaseTests")]
    public class OrderDatabaseTests
    {
        [Fact]
        public void OrdersCanBeWrittenToDatabaseWithExistingUser()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var existingOrders = context.Orders.ToList();

                    existingOrders.Count().ShouldBe(0);
                }

                using (var context = new ApplicationDbContext(options))
                {
                    context.Users.Add(CreateValidEntities.User(5));
                    context.SaveChanges();

                    var order = CreateValidEntities.Order(1);
                    order.Creator = context.Users.FirstOrDefault();
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var updatedOrders = context.Orders.Include(a => a.Creator).ToList();
                    var updatedUsers = context.Users.ToList();
                    updatedOrders.Count().ShouldBe(1);
                    updatedUsers.Count().ShouldBe(1);

                    updatedOrders[0].Creator.FirstName.ShouldBe("FirstName5");
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void OrdersCanBeWrittenToDatabaseWithNewUser()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var existingOrders = context.Orders.ToList();
                    existingOrders.Count().ShouldBe(0);
                    var existingUsers = context.Users.ToList();
                    existingUsers.Count().ShouldBe(0);
                }

                using (var context = new ApplicationDbContext(options))
                {
                    context.Users.Add(CreateValidEntities.User(5));
                    context.SaveChanges();

                    var order = CreateValidEntities.Order(1);
                    order.Creator = CreateValidEntities.User(3);
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var updatedOrders = context.Orders.Include(a => a.Creator).ToList();
                    var updatedUsers = context.Users.ToList();
                    updatedOrders.Count().ShouldBe(1);
                    updatedUsers.Count().ShouldBe(2);

                    updatedOrders[0].Creator.FirstName.ShouldBe("FirstName3");
                }
            }
            finally
            {
                connection.Close();
            }
        }



        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void OrdersCanBeWrittenToDatabaseWithTheory(int value)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var xxx = context.Orders.ToList();

                    xxx.Count().ShouldBe(0);
                }

                using (var context = new ApplicationDbContext(options))
                {
                    context.Users.Add(CreateValidEntities.User(1));
                    context.SaveChanges();

                    for (int i = 0; i < value; i++)
                    {

                        var order = CreateValidEntities.Order(i+1);
                        order.Creator = context.Users.FirstOrDefault();
                        context.Orders.Add(order);
                    }


                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var xxx = context.Orders.ToList();

                    xxx.Count().ShouldBe(value);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task TestTestAsyncSave()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var existingOrders = await context.Orders.ToListAsync();

                    existingOrders.Count().ShouldBe(0);
                }

                using (var context = new ApplicationDbContext(options))
                {
                    await context.Users.AddAsync(CreateValidEntities.User(1));
                    await context.SaveChangesAsync();

                    var order = CreateValidEntities.Order(1);
                    order.Creator = await context.Users.FirstOrDefaultAsync();
                    await context.Orders.AddAsync(order);
                    await context.SaveChangesAsync();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var updatedOrders = await context.Orders.ToListAsync();

                    updatedOrders.Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
