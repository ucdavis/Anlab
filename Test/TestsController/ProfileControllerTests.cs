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
using Microsoft.AspNetCore.Http;
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
            var user = CreateValidEntities.User(2);
            user.Id = "1";
            // Arrange
            var data = new List<User>
            {
                CreateValidEntities.User(1),
                user,
                CreateValidEntities.User(3)
            }.AsQueryable();

            //Mock context for Database
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Users).Returns(data.MockAsyncDbSet().Object);

            var user2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }));

            //For Auth? Will need to test
            //var mockPrincipal = new Mock<IPrincipal>();
            //mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            //To return the user so can check identity.
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(user2);
            
            var controller = new ProfileController(mockContext.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var controllerResult = await controller.Index();

            // Assert		
            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<User>(result.Model);
            model.FirstName.ShouldBe("FirstName2");
        }

    }
}
