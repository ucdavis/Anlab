using System;
using System.Collections.Generic;
using System.Text;
using AnlabMvc.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test.Helpers
{
    public class ContextHelper
    {
        public static ApplicationDbContext CreateContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
