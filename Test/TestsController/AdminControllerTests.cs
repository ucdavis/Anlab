using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Roles;
using AnlabMvc.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Shouldly;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class AdminControllerTests
    {
        public Mock<ApplicationDbContext> MockDbContext { get; set; }
        public Mock<HttpContext> MockHttpContext { get; set; }

        public Mock<FakeUserManager> MockUserManager { get; set; }

        public Mock<FakeRoleManager> MockRolemanager { get; set; }

        //Setup Data
        public List<User> UserData { get; set; }


        //Controller
        public AdminController Controller { get; set; }

        public AdminControllerTests()
        {
            MockDbContext = new Mock<ApplicationDbContext>();
            MockHttpContext = new Mock<HttpContext>();

            MockUserManager = new Mock<FakeUserManager>();
            MockRolemanager = new Mock<FakeRoleManager>();


            var mockDataProvider = new Mock<SessionStateTempDataProvider>();


            //Default Data
            UserData = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                var user = CreateValidEntities.User(i + 1, true);
                UserData.Add(user);
            }


            //Setups
            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockDbSet().Object);

            Controller = new AdminController(MockDbContext.Object, MockUserManager.Object, MockRolemanager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = MockHttpContext.Object
                },
                TempData = new TempDataDictionary(MockHttpContext.Object, mockDataProvider.Object)
            };
        }

        #region Index
        
        [Fact(Skip = "Changing how these are done")]
        public async Task TestIndexReturnsViewWithExpectedResults1()
        {
            // Arrange
            //All 5 users are in all roles.
            MockUserManager.Setup(a => a.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            
            // Act
            var controllerResult = await Controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<UserRolesModel>>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(5);
            foreach (var userRolesModel in modelResult)
            {
                userRolesModel.IsAdmin.ShouldBeTrue();
                userRolesModel.IsLabUser.ShouldBeTrue();
                userRolesModel.IsReports.ShouldBeTrue();
            }
        }

        [Fact(Skip = "Changing how these are done")]
        public async Task TestIndexReturnsViewWithExpectedResults2()
        {
            // Arrange            
            MockUserManager.Setup(a => a.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);
            MockUserManager.Setup(a => a.IsInRoleAsync(UserData[1], It.IsAny<string>())).ReturnsAsync(true);
            MockUserManager.Setup(a => a.IsInRoleAsync(UserData[2], RoleCodes.Admin)).ReturnsAsync(true);

            // Act
            var controllerResult = await Controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<UserRolesModel>>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(5);
            modelResult.Single(a => a.User.Id == UserData[1].Id).IsLabUser.ShouldBeTrue();
            modelResult.Single(a => a.User.Id == UserData[1].Id).IsAdmin.ShouldBeTrue();
            modelResult.Single(a => a.User.Id == UserData[1].Id).IsReports.ShouldBeTrue();

            modelResult.Single(a => a.User.Id == UserData[2].Id).IsLabUser.ShouldBeFalse();
            modelResult.Single(a => a.User.Id == UserData[2].Id).IsAdmin.ShouldBeTrue();
            modelResult.Single(a => a.User.Id == UserData[2].Id).IsReports.ShouldBeFalse();

            foreach (var ur in modelResult.Where(a => a.User.Id != UserData[1].Id && a.User.Id != UserData[2].Id).ToArray())
            {
                ur.IsReports.ShouldBeFalse();
                ur.IsAdmin.ShouldBeFalse();
                ur.IsLabUser.ShouldBeFalse();
            }

            MockUserManager.Verify(a => a.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Exactly(15));
        }
        #endregion Index

        #region EditAdmin

        [Fact]
        public async Task TestEditAdminReturnsNotFound()
        {
            // Arrange
            
            // Act
            var controllerResult = await Controller.EditAdmin("XXX");

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestEditAdminReturnsExpectedResults()
        {
            // Arrange
            MockUserManager.Setup(a => a.IsInRoleAsync(UserData[1], RoleCodes.LabUser)).ReturnsAsync(true);

            // Act
            var controllerResult = await Controller.EditAdmin(UserData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<UserRolesModel>(viewResult.Model);
            modelResult.User.Id.ShouldBe(UserData[1].Id);
            modelResult.IsLabUser.ShouldBeTrue();
            modelResult.IsAdmin.ShouldBeFalse();
            modelResult.IsReports.ShouldBeFalse();

            MockUserManager.Verify(a => a.IsInRoleAsync(UserData[1], It.IsAny<string>()), Times.Exactly(3));
        }
        

        #endregion EditAdmin
    }

    [Trait("Category", "Controller Reflection")]
    public class AdminControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public AdminControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        protected readonly Type ControllerClass = typeof(AdminController);

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
            result.Count().ShouldBe(3);

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
            result.ElementAt(0).Roles.ShouldBe($"{RoleCodes.Admin},{RoleCodes.LabUser}");
            #endregion Assert
        }

        /// <summary>
        /// #3
        /// </summary>
        [Fact]
        public void TestControllerHasAutoValidateAntiforgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "AutoValidateAntiforgeryTokenAttribute not found.");

            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        [Fact]//(Skip = "Tests are still being written. When done, remove this line.")]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            result.Count().ShouldBe(10);

            #endregion Assert
        }

        #endregion Controller Method Tests
    }
}
