using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
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
using Microsoft.AspNetCore.Mvc.Internal;
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
        public Mock<ClaimsPrincipal> MockClaimsPrincipal { get; set; }

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

            MockClaimsPrincipal = new Mock<ClaimsPrincipal>();


            var mockDataProvider = new Mock<SessionStateTempDataProvider>();


            //Default Data
            UserData = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                var user = CreateValidEntities.User(i + 1, true);
                UserData.Add(user);
            }

            var userIdent = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserData[2].Id),
            }));


            //Setups
            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockAsyncDbSet().Object);

            MockClaimsPrincipal.Setup(a => a.Claims).Returns(userIdent.Claims);
            MockClaimsPrincipal.Setup(a => a.IsInRole(RoleCodes.Admin)).Returns(false);
            MockClaimsPrincipal.Setup(a => a.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, UserData[3].Id));

            MockHttpContext.Setup(m => m.User).Returns(MockClaimsPrincipal.Object);

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
        
        [Fact]
        public async Task TestIndexReturnsViewWithExpectedResults1()
        {
            // Arrange
            var adminUsers = new List<User>();
            adminUsers.Add(UserData[1]);
            adminUsers.Add(UserData[3]);

            var labUsers = new List<User>();
            labUsers.Add(UserData[1]);
            labUsers.Add(UserData[2]);

            var reportUsers = new List<User>();
            reportUsers.Add(UserData[1]);
            reportUsers.Add(UserData[4]);

            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.Admin)).ReturnsAsync(adminUsers);
            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.LabUser)).ReturnsAsync(labUsers);
            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.Reports)).ReturnsAsync(reportUsers);
            
            
            // Act
            var controllerResult = await Controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<UserRolesModel>>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(4);

            var user = modelResult.SingleOrDefault(a => a.User.Id == UserData[1].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeTrue();
            user.IsLabUser.ShouldBeTrue();
            user.IsReports.ShouldBeTrue();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[2].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeFalse();
            user.IsLabUser.ShouldBeTrue();
            user.IsReports.ShouldBeFalse();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[3].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeTrue();
            user.IsLabUser.ShouldBeFalse();
            user.IsReports.ShouldBeFalse();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[4].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeFalse();
            user.IsLabUser.ShouldBeFalse();
            user.IsReports.ShouldBeTrue();
        }

        [Fact]
        public async Task TestIndexReturnsViewWithExpectedResults2()
        {
            // Arrange
            var adminUsers = new List<User>();

            var labUsers = new List<User>();
            labUsers.Add(UserData[2]);

            var reportUsers = new List<User>();
            reportUsers.Add(UserData[4]);

            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.Admin)).ReturnsAsync(adminUsers);
            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.LabUser)).ReturnsAsync(labUsers);
            MockUserManager.Setup(a => a.GetUsersInRoleAsync(RoleCodes.Reports)).ReturnsAsync(reportUsers);


            // Act
            var controllerResult = await Controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<UserRolesModel>>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(2);

            var user = modelResult.SingleOrDefault(a => a.User.Id == UserData[1].Id);
            user.ShouldBeNull();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[2].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeFalse();
            user.IsLabUser.ShouldBeTrue();
            user.IsReports.ShouldBeFalse();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[3].Id);
            user.ShouldBeNull();

            user = modelResult.SingleOrDefault(a => a.User.Id == UserData[4].Id);
            user.ShouldNotBeNull();
            user.IsAdmin.ShouldBeFalse();
            user.IsLabUser.ShouldBeFalse();
            user.IsReports.ShouldBeTrue();
        }
        #endregion Index

        #region SearchAdminUser

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestSearchAdminUserRedirectsToIndex1(string value)
        {
            // Arrange
            
            // Act
            var controllerResult = await Controller.SearchAdminUser(value);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Nothing entered to search.");
        }

        [Fact]
        public async Task TestSearchAdminUserRedirectsToIndex2()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.SearchAdminUser("xxx");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Email xxx not found.");
        }

        [Theory]
        [InlineData("xxx@xx.com")]
        [InlineData("XXX@XX.COM")]
        [InlineData(" XXX@XX.COM ")]
        public async Task TestSearchAdminUserRedirectsToEditAdmin(string value)
        {
            // Arrange
            UserData[1].NormalizedUserName = "XXX@XX.COM";
            // Act
            var controllerResult = await Controller.SearchAdminUser(value);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("EditAdmin");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(UserData[1].Id);

            Controller.ErrorMessage.ShouldBeNull();
        }

        [Fact]
        public async Task TestSearchAdminUserThrowsExceptionIfDuplicate()
        {
            // Arrange
            UserData[1].NormalizedUserName = "XXX@XX.COM"; 
            UserData[2].NormalizedUserName = "XXX@XX.COM"; //Should never happen...

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await Controller.SearchAdminUser("xxx@xx.com"));

            // Assert
            ex.ShouldNotBeNull();
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }

        #endregion SearchAdminUser

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

        #region ListClients

        [Fact]
        public async Task TestListClientsReturnsView()
        {
            // Arrange
            
            // Act
            var controllerResult = await Controller.ListClients();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<User>>(viewResult.Model);

            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(5);
        }


        #endregion Description

        #region EditUser

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("xxx")]
        public async Task TestEditUserGetRedirectsWhenUserNotFound(string value)
        {
            // Arrange
            
            // Act
            var controllerResult = await Controller.EditUser(value);

            // Assert
            var redrectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redrectResult.ActionName.ShouldBe("ListClients");
            redrectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("User Not Found.");
        }

        [Fact]
        public async Task TestEditUserGetReturnsView()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.EditUser(UserData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<User>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Id.ShouldBe(UserData[1].Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("xxx")]
        public async Task TestEditUserPostRedirectsWhenUserNotFound(string value)
        {
            // Arrange

            // Act
            var controllerResult = await Controller.EditUser(value, CreateValidEntities.User(7));

            // Assert
            var redrectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redrectResult.ActionName.ShouldBe("ListClients");
            redrectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("User Not Found.");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            MockDbContext.Verify(a => a.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task TestEditUserPostThrowsExceptionIfIdDoesNotMatch()
        {
            // Arrange
            var model = CreateValidEntities.User(7);
            //model.Id = UserData[1].Id;
            Controller.ModelState.AddModelError("Fake", "Fake Error");

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Controller.EditUser(UserData[1].Id, model));

            // Assert
            ex.ShouldNotBeNull();
            ex.Message.ShouldBe("User id did not match passed value.");
        }

        [Fact]
        public async Task TestEditUserPostReturnsViewIfModelStateInvalid()
        {
            // Arrange
            var model = CreateValidEntities.User(7);
            model.Id = UserData[1].Id;
            Controller.ModelState.AddModelError("Fake", "Fake Error");

            // Act
            var controllerResult = await Controller.EditUser(UserData[1].Id, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<User>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Id.ShouldBe(UserData[1].Id);

            Controller.ErrorMessage.ShouldBe("The user had invalid data.");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            MockDbContext.Verify(a => a.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task TestEditUserPostSavesExpectedFields()
        {
            // Arrange
            var model = CreateValidEntities.User(7, true);
            model.Id = UserData[1].Id;

            // Act
            var controllerResult = await Controller.EditUser(UserData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ShouldNotBeNull();
            redirectResult.ActionName.ShouldBe("ListClients");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("User Updated.");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockDbContext.Verify(a => a.Update(UserData[1]), Times.Once);

            //Fields
            UserData[1].FirstName.ShouldBe("FirstName7");
            UserData[1].LastName.ShouldBe("LastName7");
            UserData[1].Name.ShouldBe("FirstName7 LastName7");
            UserData[1].Phone.ShouldBe("Phone7");
            UserData[1].Account.ShouldBe("ACCOUNT7");
            UserData[1].ClientId.ShouldBe("CLIENTID7");
            UserData[1].CompanyName.ShouldBe("CompanyName7");
            UserData[1].BillingContactName.ShouldBe("BillingContactName7");
            UserData[1].BillingContactAddress.ShouldBe("BillingContactAddress7");
            UserData[1].BillingContactEmail.ShouldBe("BillingContactEmail7@test.com");
            UserData[1].BillingContactPhone.ShouldBe("BillingContactPhone7");

            UserData[1].NormalizedUserName.ShouldBe("NormalizedUserName2"); //Unchanged
        }

        #endregion EditUser

        #region AddUserToRole

        [Theory]
        [InlineData(RoleCodes.Admin, true)]
        [InlineData(RoleCodes.Admin, false)]
        [InlineData(RoleCodes.LabUser, true)]
        [InlineData(RoleCodes.LabUser, false)]
        [InlineData(RoleCodes.Reports, true)]
        [InlineData(RoleCodes.Reports, false)]
        public async Task TestAddUserToRoleThrowsExceptionWhenUserNotFound(string role, bool add)
        {
            // Arrange
            
            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await Controller.AddUserToRole("xxx", role, add));

            // Assert
            ex.ShouldNotBeNull();
            ex.Message.ShouldBe("Sequence contains no matching element");

            MockUserManager.Verify(a => a.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            MockUserManager.Verify(a => a.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(RoleCodes.Admin)]
        [InlineData(RoleCodes.LabUser)]
        [InlineData(RoleCodes.Reports)]
        public async Task TestAddUserToRoleWhenAdd(string role)
        {
            // Arrange
            

            // Act
            var controllerResult = await Controller.AddUserToRole(UserData[1].Id, role, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ShouldNotBeNull();
            redirectResult.ActionName.ShouldBe("EditAdmin");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(UserData[1].Id);

            MockUserManager.Verify(a => a.AddToRoleAsync(UserData[1], role), Times.Once);
            MockUserManager.Verify(a => a.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);

            Controller.ErrorMessage.ShouldBeNull();
        }

        [Theory]
        [InlineData(RoleCodes.Admin)]
        [InlineData(RoleCodes.LabUser)]
        [InlineData(RoleCodes.Reports)]
        public async Task TestAddUserToRoleWhenRemoveOwnRole(string role)
        {
            // Arrange
            //UserData[3] is configured above to be the CurrentUser


            // Act
            var controllerResult = await Controller.AddUserToRole(UserData[3].Id, role, false);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ShouldNotBeNull();
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Can't remove your own permissions.");

            MockUserManager.Verify(a => a.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            MockUserManager.Verify(a => a.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);

        }

        [Theory]
        [InlineData(RoleCodes.Admin)]
        [InlineData(RoleCodes.LabUser)]
        [InlineData(RoleCodes.Reports)]
        public async Task TestAddUserToRoleWhenRemoveOtherRole(string role)
        {
            // Arrange
            //UserData[3] is configured above to be the CurrentUser


            // Act
            var controllerResult = await Controller.AddUserToRole(UserData[2].Id, role, false);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ShouldNotBeNull();
            redirectResult.ActionName.ShouldBe("EditAdmin");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(UserData[2].Id);

            Controller.ErrorMessage.ShouldBeNull();

            MockUserManager.Verify(a => a.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            MockUserManager.Verify(a => a.RemoveFromRoleAsync(UserData[2], role), Times.Once);

        }

        #endregion AddUserToRole

        #region MailQueue

        [Fact]
        public async Task TestMailQueueReturnsView1()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 10; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mail.Add(mm);
            }

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);

            for (int i = 0; i < 3; i++)
            {
                mail[i].Order = CreateValidEntities.Order(2);
            }

            // Act
            var controllerResult = await Controller.MailQueue(mail[0].Order.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<MailMessage>>(viewResult.Model);

            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(3);
            modelResult[0].Id.ShouldBe(1);
            modelResult[0].Order.Id.ShouldBe(2);
        }

        [Fact]
        public async Task TestMailQueueReturnsView2()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 10; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mm.Sent = null;
                mm.SentAt = DateTime.UtcNow.AddDays(-35);
                mail.Add(mm);
            }

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);

            // Act
            var controllerResult = await Controller.MailQueue();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<MailMessage>>(viewResult.Model);

            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(10);
        }

        [Fact]
        public async Task TestMailQueueReturnsView3()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 10; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mm.Sent = false;
                mm.SentAt = DateTime.UtcNow.AddDays(-35);
                mail.Add(mm);
            }

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);

            // Act
            var controllerResult = await Controller.MailQueue();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<MailMessage>>(viewResult.Model);

            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(10);
        }

        [Fact]
        public async Task TestMailQueueReturnsView4()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 10; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mm.Sent = true;
                mm.SentAt = DateTime.UtcNow.AddDays(-35);
                mail.Add(mm);
            }

            mail[1].SentAt = DateTime.UtcNow.AddDays(-28);

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);

            // Act
            var controllerResult = await Controller.MailQueue();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<MailMessage>>(viewResult.Model);

            modelResult.ShouldNotBeNull();
            modelResult.Count.ShouldBe(1);
        }

        #endregion MailQueue

        #region ViewMessage

        [Fact]
        public async Task TestViewMessageReturnsNotFound()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 5; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mm.Sent = null;
                mm.SentAt = DateTime.UtcNow.AddDays(-35);
                mail.Add(mm);
            }

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);



            // Act
            var controllerResult = await Controller.ViewMessage(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestViewMessageReturnsView()
        {
            // Arrange
            var mail = new List<MailMessage>();
            for (int i = 0; i < 5; i++)
            {
                var mm = CreateValidEntities.MailMessage(i + 1);
                mm.Order = CreateValidEntities.Order(i + 1);
                mm.User = UserData[i % 2];
                mm.Sent = null;
                mm.SentAt = DateTime.UtcNow.AddDays(-35);
                mail.Add(mm);
            }

            MockDbContext.Setup(a => a.MailMessages).Returns(mail.AsQueryable().MockAsyncDbSet().Object);



            // Act
            var controllerResult = await Controller.ViewMessage(3);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<MailMessage>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Id.ShouldBe(3);
        }
        #endregion ViewMessage
    }

    [Trait("Category", "Controller Reflection")]
    public class AdminControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public AdminControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(AdminController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(3);
            authAttribute.ElementAt(0).Roles.ShouldBe($"{RoleCodes.Admin},{RoleCodes.LabUser}");
            
            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(3);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(11);
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
            var indexAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("Index", 2 + countAdjustment, "Index-1", false, showListOfAttributes: false);
            indexAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Index", 2 + countAdjustment, "Index-1", false, showListOfAttributes: false);

            //2
            var searchAdminUserAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("SearchAdminUser", 3 + countAdjustment, "SearchAdminUser-1", false, showListOfAttributes: false);
            searchAdminUserAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("SearchAdminUser", 3 + countAdjustment, "SearchAdminUser-2", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("SearchAdminUser", 3 + countAdjustment, "SearchAdminUser-3", false, showListOfAttributes: false);

            //3
            var editAdminAminUserAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("EditAdmin", 3 + countAdjustment, "EditAdmin-1", false, showListOfAttributes: false);
            editAdminAminUserAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("SearchAdminUser", 3 + countAdjustment, "EditAdmin-2", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("EditAdmin", 3 + countAdjustment, "EditAdmin-3", false, showListOfAttributes: false);

            //4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ListClients", 1 + countAdjustment, "ListClients-1", false, showListOfAttributes: false);

            //5 & 6
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("EditUser", 1 + countAdjustment, "EditUserGet-1", false, showListOfAttributes: false);

            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("EditUser", 2 + countAdjustment, "EditUserPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("EditUser", 2 + countAdjustment, "EditUserPost-2", true, showListOfAttributes: false);

            //7
            var addUserToRoleAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("AddUserToRole", 3 + countAdjustment, "AddUserToRole-1", false, showListOfAttributes: false);
            addUserToRoleAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("AddUserToRole", 3 + countAdjustment, "AddUserToRole-2", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("AddUserToRole", 3 + countAdjustment, "AddUserToRole-3", false, showListOfAttributes: false);

            //8 
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("MailQueue", 1 + countAdjustment, "MailQueue-1", false, showListOfAttributes: false);

            //9
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ViewMessage", 1 + countAdjustment, "ViewMessage-1", false, showListOfAttributes: false);

            //10 & 11
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("FixEmail", 1 + countAdjustment, "FixEmailGet-1", false, showListOfAttributes: false);

            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("FixEmail", 2 + countAdjustment, "FixEmailPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("FixEmail", 2 + countAdjustment, "FixEmailPost-2", true, showListOfAttributes: false);

        }
    }
}
