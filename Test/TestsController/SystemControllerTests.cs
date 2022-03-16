using System;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AnlabMvc.Services;
using Moq;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class SystemControllerTests
    {
        public Mock<IDbInitializationService> MockDbIntService { get; set; }
        public SystemController Controller { get; set; }

        public SystemControllerTests()
        {
            MockDbIntService = new Mock<IDbInitializationService>();
            Controller = new SystemController(MockDbIntService.Object);
        }

        [Fact]
        public Task TestResetDbThrowsException()
        {
            // Arrange

            // Act
            var ex = Assert.ThrowsAsync<NotImplementedException>(async () => await Controller.ResetDb());
            // Assert
#if DEBUG
            ex.Result.Message.ShouldBe("Only enable this when working against a local database.");
#else
            ex.Result.Message.ShouldBe("WHAT!!! Don't reset DB in Release!");
#endif
            MockDbIntService.Verify(a => a.RecreateAndInitialize(), Times.Never);

            return Task.CompletedTask;
        }
    }

    [Trait("Category", "Controller Reflection")]
    public class SystemControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public SystemControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(SystemController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(3);
            authAttribute.ElementAt(0).Roles.ShouldNotBeNull();
            authAttribute.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(3);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(1);
        }
    }
}
