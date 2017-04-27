using System.Linq;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Shouldly;
using Test.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category", "DatabaseTests")]
    public class OrderDatabaseTests
    {
        [Fact]
        public void test1()
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
                    context.Users.Add(CreateValidEntities.User(1));
                    context.SaveChanges();

                    var order = CreateValidEntities.Order(1);
                    order.Creator = context.Users.FirstOrDefault();
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var xxx = context.Orders.ToList();

                    xxx.Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void test2()
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
                    context.Users.Add(CreateValidEntities.User(1));
                    context.SaveChanges();

                    var order = CreateValidEntities.Order(1);
                    order.Creator = context.Users.FirstOrDefault();
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var xxx = context.Orders.ToList();

                    xxx.Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
