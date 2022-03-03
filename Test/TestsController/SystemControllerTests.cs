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
#if DEBUG
        [Fact]
        public async Task TestResetDbCallsService()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.ResetDb();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("LogoutDirect");
            redirectResult.ControllerName.ShouldBe("Account");

            MockDbIntService.Verify(a => a.RecreateAndInitialize(), Times.Once);
        }
#else
        [Fact]
        public Task TestResetDbThrowsException()
        {
            // Arrange

            // Act
            var ex = Assert.ThrowsAsync<NotImplementedException>(async () => await Controller.ResetDb());
            // Assert
            ex.Result.Message.ShouldBe("WHAT!!! Don't reset DB in Release!");
            MockDbIntService.Verify(a => a.RecreateAndInitialize(), Times.Never);

            return Task.CompletedTask;
        }
#endif

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

        [Fact]
        public void TestControllerMethodAttributes()
        {

#if DEBUG
            var countAdjustment = 1;
#else
            var countAdjustment = 0;
#endif
            //1
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ResetDb", 1 + countAdjustment, "ResetDb-1", showListOfAttributes: false);

        }

    }
}
