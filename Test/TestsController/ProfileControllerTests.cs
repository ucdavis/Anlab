using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;
using AnlabMvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using Test.Helpers;
using Xunit;


namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class ProfileControllerTests
    {
        [Fact]
        public async Task TestWithMoqDb()
        {
            //var context = new Mock<HttpContextBase>();
            //var mockIdentity = new Mock<IIdentity>();
            //context.SetupGet(x => x.User.Identity).Returns(mockIdentity.Object);
            //mockIdentity.Setup(x => x.Name).Returns("test_name");

            // Arrange
            var data = new List<User>
            {
                CreateValidEntities.User(1),
                CreateValidEntities.User(2),
                CreateValidEntities.User(3)
            }.AsQueryable();


            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Users).Returns(data.MockAsyncDbSet().Object);


            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }));

            var controller = new ProfileController(mockContext.Object)
            {
                HttpContext = { User = currentUser }
            };

            // Act
            var controllerResult = await controller.Index();

            // Assert		
            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<User>(result.Model);
            model.FirstName.ShouldBe("FirstName1");
        }

        [Fact]
        public async Task TestWithInMemoryDb()
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
                    await context.Users.AddAsync(CreateValidEntities.User(1));
                    await context.Users.AddAsync(CreateValidEntities.User(2));
                    await context.Users.AddAsync(CreateValidEntities.User(3));
                    await context.SaveChangesAsync();

                }

                using (var context = new ApplicationDbContext(options))
                {
                    var controller = new ProfileController(context);
                    var controllerResult = await controller.Index();

                    // Assert		
                    var result = Assert.IsType<ViewResult>(controllerResult);
                    var model = Assert.IsType<User>(result.Model);
                    model.FirstName.ShouldBe("FirstName1");
                }
            }
            finally
            {
                connection.Close();
            }




        }
    }
}
