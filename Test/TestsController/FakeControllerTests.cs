using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;
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
        public Mock<UserManager<User>> MockUserManager { get; set; }

        public Mock<FakeUserManager> fakeUserM { get; set; }

        public List<User> UserData { get; set; }
        public FakeController Controller { get; set; }

        public FakeControllerTests()
        {
            fakeUserM = new Mock<FakeUserManager>();


            MockDbContext = new Mock<ApplicationDbContext>();
            MockUserManager = new Mock<UserManager<User>>();
            UserData = new List<User>()
            {
                CreateValidEntities.User(1, true),
                CreateValidEntities.User(2, true)
            };
            UserData[0].Id = "Creator1";

            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockAsyncDbSet().Object);

            //Controller = new FakeController(new FakeUserManager());
            Controller = new FakeController(fakeUserM.Object);
        }

        [Fact]
        public async Task TestDescription()
        {
            // Arrange
            


            // Act
            var controllerresult = await Controller.Index();

            // Assert
            fakeUserM.Verify(a => a.FindByNameAsync("test@test.com"), Times.Once);
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

        public override Task<User> FindByEmailAsync(string email)
        {
            return Task.FromResult(new User { Email = email });
        }

        public override Task<User> FindByNameAsync(string email)
        {
            return Task.FromResult(new User { Email = email });
        }

        public override Task<bool> IsEmailConfirmedAsync(User user)
        {
            return Task.FromResult(user.Email == "test@test.com");
        }

        public override Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return Task.FromResult("---------------");
        }

    }
}
