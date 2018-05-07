using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class FakeControllerTests
    {
        public Mock<ApplicationDbContext> MockDbContext { get; set; }

        public Mock<FakeUserManager> FakeUserManager { get; set; }
        public Mock<FakeSignInManager> FakeSignInmanager { get; set; }

        public List<User> UserData { get; set; }
        public FakeController Controller { get; set; }

        public FakeControllerTests()
        {
            FakeUserManager = new Mock<FakeUserManager>();
            FakeSignInmanager = new Mock<FakeSignInManager>();


            MockDbContext = new Mock<ApplicationDbContext>();
            UserData = new List<User>()
            {
                CreateValidEntities.User(1, true),
                CreateValidEntities.User(2, true)
            };
            UserData[0].Id = "Creator1";

            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockAsyncDbSet().Object);

            //Controller = new FakeController(new FakeUserManager());
            Controller = new FakeController(FakeUserManager.Object, FakeSignInmanager.Object);
        }

        [Fact]
        public async Task TestDescription()
        {
            // Arrange
            var user = CreateValidEntities.User(3);
            FakeUserManager.Setup(a => a.IsInRoleAsync(It.IsAny<User>(), "Test")).ReturnsAsync(true);
            FakeUserManager.Setup(a => a.FindByNameAsync("test@test.com")).ReturnsAsync(user);

            // Act
            var controllerresult = await Controller.Index();

            // Assert
            FakeUserManager.Verify(a => a.FindByNameAsync("test@test.com"), Times.Once);
            FakeSignInmanager.Verify(a => a.SignOutAsync(), Times.Once);
            FakeSignInmanager.Verify(a => a.SignInAsync(user, false, It.IsAny<string>()), Times.Once);
        }
    }

    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object)
        { }
    }

    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager()
            : base(new Mock<FakeUserManager>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object)
        { }
    }

}
