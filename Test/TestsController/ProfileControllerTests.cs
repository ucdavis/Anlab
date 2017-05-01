using System;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;
using AnlabMvc.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Test.Helpers;
using Xunit;
using Xunit.Abstractions;


namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class ProfileControllerTests
    {
        

        [Fact]
        public async Task TestWithMoqDb()
        {
            var user = CreateValidEntities.User(2);
            user.Id = "44";
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
                new Claim(ClaimTypes.NameIdentifier, "44"),
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

    [Trait("Category", "Controller Reflection")]
    public class ProfileControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ProfileControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        protected readonly Type ControllerClass = typeof(ProfileController);

        #region Controller Class Tests
        [Fact]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            controllerClass.BaseType.ShouldNotBe(null);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            result.ShouldBe("ApplicationController");

            #endregion Assert
        }

        [Fact]
        public void TestControllerExpectedNumberOfAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in result)
            {
                output.WriteLine(o.ToString()); //Output shows 
            }
            result.Count().ShouldBe(2);

            #endregion Assert
        }

        /// <summary>
        /// #1
        /// </summary>
        [Fact]
        public void TestControllerHasControllerAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ControllerAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "ControllerAttribute not found.");

            #endregion Assert
        }

        /// <summary>
        /// #2
        /// </summary>
        [Fact]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "AuthorizeAttribute not found.");

            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        [Fact(Skip = "Tests are still being written. When done, remove this line.")]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            result.Count().ShouldBe(1);

            #endregion Assert
        }
#if DEBUG
        [Fact]
        public void TestControllerMethodIndexContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<DebuggerStepThroughAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in allAttributes)
            {
                output.WriteLine(o.ToString()); //Output shows if the test fails
            }
            allAttributes.Count().ShouldBe(2, "No Attributes");
            expectedAttribute.Count().ShouldBe(1, "DebuggerStepThroughAttribute not found");
            #endregion Assert
        }
#endif
        [Fact]
        public void TestControllerMethodIndexContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AsyncStateMachineAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in allAttributes)
            {
                output.WriteLine(o.ToString()); //Output shows if the test fails
            }
#if DEBUG        
            allAttributes.Count().ShouldBe(2, "No Attributes");
#else
            allAttributes.Count().ShouldBe(1, "No Attributes");
#endif
            expectedAttribute.Count().ShouldBe(1, "AsyncStateMachineAttribute not found");
            #endregion Assert
        }

#endregion Controller Method Tests
    }


}
