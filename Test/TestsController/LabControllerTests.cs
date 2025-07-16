using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Models.AggieEnterpriseModels;
using Anlab.Core.Services;
using Anlab.Jobs.MoneyMovement;
using AnlabMvc;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class LabControllerTests
    {
        public Mock<ApplicationDbContext> MockDbContext { get; set; }
        public Mock<HttpContext> MockHttpContext { get; set; }
        public Mock<IOrderService> MockOrderService { get; set; }
        public Mock<ILabworksService> MockLabworksService { get; set; }
        public Mock<IOrderMessageService> MockOrderMessagingService { get; set; }
        public Mock<IFileStorageService> MockFileStorageService { get; set; }
        public Mock<ISlothService> MockSlothService { get; set; }
        public Mock<ClaimsPrincipal> MockClaimsPrincipal { get; set; }
        public Mock<IFinancialService> MockFinancialService { get; set; }
        public Mock<TempDataSerializer> MockTempDataSerializer { get; set; }

        public Mock<IFormFile> MockFormFile { get; set; }

        public Mock<IAggieEnterpriseService> MockAggieEnterpriseService { get; set; }
        public Mock<IOptions<AggieEnterpriseSettings>> MockAeSettings { get; set; }
        public Mock<IOptions<AppSettings>> MockAppSettings { get; set; }

        public AggieEnterpriseSettings AeSettings { get; set; } = new AggieEnterpriseSettings()
        {
            UseCoA = false,
            GraphQlUrl = "http://fake.ucdavis.edu/graphql",
            Token = "Fake"
        };

        public AppSettings AppSettings { get;set; } = new AppSettings()
        {
            AllowDuplicateRequestNums = true,
        };


        //Setup Data
        public List<Order> OrderData { get; set; }
        public List<User> UserData { get; set; }

        //Controller
        public LabController Controller { get; set; }





        public LabControllerTests()
        {
            MockDbContext = new Mock<ApplicationDbContext>();
            MockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            MockHttpContext = new Mock<HttpContext>();
            MockOrderService = new Mock<IOrderService>();
            MockLabworksService = new Mock<ILabworksService>();
            MockOrderMessagingService = new Mock<IOrderMessageService>();
            MockFileStorageService = new Mock<IFileStorageService>();
            MockSlothService = new Mock<ISlothService>();
            MockFormFile = new Mock<IFormFile>();
            MockFinancialService = new Mock<IFinancialService>();
            MockTempDataSerializer = new Mock<TempDataSerializer>();
            MockAggieEnterpriseService = new Mock<IAggieEnterpriseService>();

            var mockDataProvider = new Mock<SessionStateTempDataProvider>(MockTempDataSerializer.Object);

            MockAeSettings = new Mock<IOptions<AggieEnterpriseSettings>>();
            MockAeSettings.SetupGet(x => x.Value).Returns(AeSettings);

            MockAppSettings = new Mock<IOptions<AppSettings>>();
            MockAppSettings.SetupGet(x => x.Value).Returns(AppSettings);

            

            //Default Data
            OrderData = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                var order = CreateValidEntities.Order(i + 1, true);
                order.Creator = CreateValidEntities.User(2);
                OrderData.Add(order);
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Creator1"),
            }));

            UserData = new List<User>()
            {
                CreateValidEntities.User(1, true),
                CreateValidEntities.User(2, true)
            };
            UserData[0].Id = "Creator1";

            
            //Setups
            MockClaimsPrincipal.Setup(a => a.Claims).Returns(user.Claims);
            MockClaimsPrincipal.Setup(a => a.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, "Creator1"));
            MockDbContext.Setup(m => m.Orders).Returns(OrderData.AsQueryable().MockAsyncDbSet().Object);
            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockAsyncDbSet().Object);
            MockDbContext.Setup(a => a.History).Returns(new List<History>().AsQueryable().MockAsyncDbSet().Object);
            MockDbContext.Setup(a => a.MailMessages).Returns(new List<MailMessage>().AsQueryable().MockAsyncDbSet().Object);
            MockHttpContext.Setup(m => m.User).Returns(MockClaimsPrincipal.Object);

            Controller = new LabController(MockDbContext.Object, MockOrderService.Object, MockLabworksService.Object,
                MockOrderMessagingService.Object, MockFileStorageService.Object, MockSlothService.Object, MockFinancialService.Object, MockAeSettings.Object,
                MockAggieEnterpriseService.Object, MockAppSettings.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = MockHttpContext.Object
                },
                TempData = new TempDataDictionary(MockHttpContext.Object, mockDataProvider.Object)
            };
        }

        #region Orders

        [Theory]
        [InlineData(OrderStatusCodes.Confirmed, false)]
        [InlineData(OrderStatusCodes.Received, false)]
        [InlineData(OrderStatusCodes.Finalized, false)]
        [InlineData(OrderStatusCodes.Confirmed, true)]
        [InlineData(OrderStatusCodes.Received, true)]
        [InlineData(OrderStatusCodes.Finalized, true)]
        [InlineData(OrderStatusCodes.Complete, true)]
        [InlineData(OrderStatusCodes.Complete, false)]
        public async void TestOrderReturnsExpectedResults1(string value, bool showComplete)
        {

            // Arrange
            foreach (var order in OrderData)
            {
                order.Status = OrderStatusCodes.Created;
            }

            OrderData[1].Status = value;
            OrderData[1].RequestNum = "Blah";
            OrderData[1].ClientName = "Hup";
            OrderData[2].Status = value;
            OrderData[2].Paid = true;

            // Act
            var controllerResult =  await Controller.Orders(showComplete);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<LabOrderListModel>(viewResult.Model);
            if (value == OrderStatusCodes.Complete && showComplete == false)
            {
                modelResult.Orders.Count.ShouldBe(0);
            }
            else
            {
                modelResult.Orders.Count.ShouldBe(2);
                modelResult.Orders[0].Id.ShouldBe(3); //Sorted by updated date
                modelResult.Orders[1].Id.ShouldBe(2); //Sorted by updated date

                modelResult.Orders[1].ClientId.ShouldBe("ClientId2");
                modelResult.Orders[1].Creator.ShouldNotBeNull();
                modelResult.Orders[1].Creator.Email.ShouldBe("test2@testy.com");
                modelResult.Orders[1].Created.ShouldNotBe(default);
                modelResult.Orders[1].Updated.ShouldNotBe(default);
                modelResult.Orders[1].RequestNum.ShouldBe("Blah");
                modelResult.Orders [1].Status.ShouldBe(value);
                modelResult.Orders[1].ShareIdentifier.ShouldBe(SpecificGuid.GetGuid(2));
                modelResult.Orders[1].Paid.ShouldBeFalse();

                modelResult.Orders[0].Paid.ShouldBeTrue();

                modelResult.Orders[1].ClientName.ShouldBe("Hup");
            }

            Controller.ViewBag.ShowComplete = showComplete;

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void TestOrdersReturnsMax1000Orders(bool value)
        {
            // Arrange
            for (int i = 0; i < 1200; i++)
            {
                var order = CreateValidEntities.Order(i + 10);
                order.Status = OrderStatusCodes.Finalized;
                OrderData.Add(order);
            }


            // Act
            var controllerResult = await Controller.Orders(value);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<LabOrderListModel>(viewResult.Model);
            modelResult.Orders.Count.ShouldBe(1000);
        }
        #endregion Orders

        #region Details

        [Fact]
        public async Task TestDetailsReturnsNotFound1()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Details(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }
        [Fact]
        public async Task TestDetailsReturnsNotFound2()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Created;


            // Act
            var controllerResult = await Controller.Details(OrderData[1].Id);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestDetailsReturnsView(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var controllerResult = await Controller.Details(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderReviewModel>(viewResult.Model);

            modelResult.Order.ShouldNotBeNull();
            modelResult.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderDetails.ShouldNotBeNull();
            modelResult.OrderDetails.ClientInfo.ClientId.ShouldBe("ClientId2");
            modelResult.HideLabDetails.ShouldBeFalse();
        }
        #endregion Details

        #region AddRequestNumber

        [Fact]
        public async Task TestAddRequestNumberGetReturnsNotFound()
        {
            // Arrange

            // Act
            var cr = await Controller.AddRequestNumber(9);

            // Assert
            Assert.IsType<NotFoundResult>(cr);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestAddRequestNumberGetRedirectsWhenStatusNotConfirmed(string value)
        {
            // Arrange
            OrderData[1].Status = value;


            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id);

            // Assert
            var rd = Assert.IsType<RedirectToActionResult>(cr);
            rd.ActionName.ShouldBe("Orders");
            rd.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only receive a confirmed order");
        }
        [Fact]
        public async Task TestAddRequestNumberGetReturnsView()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var controllerResult = await Controller.Details(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderReviewModel>(viewResult.Model);

            modelResult.Order.ShouldNotBeNull();
            modelResult.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderDetails.ShouldNotBeNull();
            modelResult.OrderDetails.ClientInfo.ClientId.ShouldBe("ClientId2");
            modelResult.HideLabDetails.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestAddRequestNumberPostRedirectsWhenRequestNumMissing(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, value);

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("AddRequestNumber");
            Controller.ErrorMessage.ShouldBe("A request number is required");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUppersValueAndChecksForDups1a()
        {
            // Arrange
            OrderData[0].RequestNum = "ABC123";
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            AppSettings.AllowDuplicateRequestNums = true;

            // Act
            var cr = await Controller.AddRequestNumber(9, true, "AbC123");

            // Assert
            Assert.IsType<NotFoundResult>(cr); //Because it falls through to the check if the order exists.

            Controller.ErrorMessage.ShouldBe("That request number is already in use");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUppersValueAndChecksForDups1b()
        {
            // Arrange
            OrderData[0].RequestNum = "ABC123";
            OrderData[1].Status = OrderStatusCodes.Confirmed;

            AppSettings.AllowDuplicateRequestNums = false;

            // Act
            var cr = await Controller.AddRequestNumber(9, true, "AbC123");

            // Assert

            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("AddRequestNumber");
            rr.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("That request number is already in use");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUppersValueAndChecksForDups2()
        {
            // Arrange
            OrderData[0].RequestNum = "AbC123";
            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var cr = await Controller.AddRequestNumber(9, true, OrderData[0].RequestNum);

            // Assert
            Assert.IsType<NotFoundResult>(cr); //Because it didn't find the lower case match.

            Controller.ErrorMessage.ShouldBeNull();
        }

        [Fact]
        public async Task TestAddRequestNumberPostReturnsNotFound()
        {
            // Arrange

            // Act
            var cr = await Controller.AddRequestNumber(9, true, "AAABBB");

            // Assert
            Assert.IsType<NotFoundResult>(cr);
            Controller.ErrorMessage.ShouldBeNull();
        }

        [Theory]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestAddRequestNumberWhenWrongStatus(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Orders");
            rr.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only receive a confirmed order");
        }

        [Fact]
        public async Task TestAddRequestNumberPostWhenOverwriteOrderFromDbReturnsSpecificError()
        {
            // Arrange
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ErrorMessage = "The Fake Error has arrived.";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Orders");
            rr.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. Error looking up on Labworks: The Fake Error has arrived.");
        }

        [Fact]
        public async Task TestAddRequestNumberPostWhenOverwriteOrderFromDbReturnsErrorWithMissingCodes()
        {
            // Arrange
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.MissingCodes = new List<string>();
            overWriteResult.MissingCodes.Add("A123");
            overWriteResult.MissingCodes.Add("B345");
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Orders");
            rr.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. The following codes were not found locally: A123,B345");
        }


        [Fact]
        public async Task TestAddRequestNumberPostWarnsWhenClientIdChanged()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = "FAKED";
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);
            Controller.ErrorMessage.ShouldBe("Warning!!! Client Id is changing from FAKED to NotFaked");
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestAddRequestNumberPostDoesNotWarnWhenClientIdChangedFrom(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = value;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);
            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Theory]
        [InlineData("AdditionalEmails2")]
        [InlineData("AdditionalEmails2;Email-xx")]
        public async Task TestAddRequestNumberPostUpdatesOrderAdditionalEmails(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(new ClientDetailsLookupModel());

            //Verify what it looks like before
            OrderData[1].AdditionalEmails = value;
            var od = OrderData[1].GetOrderDetails();
            od.ClientInfo = new ClientInfo();
            od.ClientInfo.Email = "Email-xx";
            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            OrderData[1].AdditionalEmails.ShouldBe("AdditionalEmails2;Email-xx");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedValues1() //Null value for GetClientDetails
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(new ClientDetailsLookupModel());

            //Verify what it looks like before
            OrderData[1].ClientName.ShouldBeNull();
            OrderData[1].ClientId.ShouldNotBe("NotFaked");
            OrderData[1].AdditionalEmails.ShouldBe("AdditionalEmails2");
            var od = OrderData[1].GetOrderDetails();
            od.ClientInfo = new ClientInfo();
            od.ClientInfo.ClientId = "ClientId-xx";
            od.ClientInfo.Email = "Email-xx";
            od.ClientInfo.Name = "Name-xx";
            od.ClientInfo.PhoneNumber = "PhoneNumber-xx";
            od.ClientInfo.CopyPhone = "CopyPhone-xx";
            od.ClientInfo.Department = "Department-xx";

            od.Quantity = 0;
            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            OrderData[1].ClientName.ShouldBe("[Not Found]");
            OrderData[1].ClientId.ShouldBe("NotFaked");
            var notUpdatedDetails = OrderData[1].GetOrderDetails();
            notUpdatedDetails.ClientInfo.ClientId.ShouldBe("ClientId-xx");
            notUpdatedDetails.ClientInfo.Email.ShouldBe("Email-xx");
            notUpdatedDetails.ClientInfo.Name.ShouldBe("Name-xx");
            notUpdatedDetails.ClientInfo.PhoneNumber.ShouldBe("PhoneNumber-xx");
            notUpdatedDetails.ClientInfo.CopyPhone.ShouldBe("CopyPhone-xx");
            notUpdatedDetails.ClientInfo.Department.ShouldBe("Department-xx");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }


        [Fact]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedValues2()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            OrderData[1].ClientName.ShouldBeNull();
            OrderData[1].ClientId.ShouldNotBe("NotFaked");
            OrderData[1].AdditionalEmails.ShouldBe("AdditionalEmails2");
            var od = OrderData[1].GetOrderDetails();
            od.ClientInfo = new ClientInfo();
            od.ClientInfo.ClientId = "ClientId-xx";
            od.ClientInfo.Email = "Email-xx";
            od.ClientInfo.Name = "Name-xx";
            od.ClientInfo.PhoneNumber = "PhoneNumber-xx";
            od.ClientInfo.CopyPhone = "CopyPhone-xx";
            od.ClientInfo.Department = "Department-xx";

            od.Quantity = 0;
            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            OrderData[1].ClientName.ShouldBe("Name5");
            OrderData[1].ClientId.ShouldBe("NotFaked");
            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.ClientInfo.ClientId.ShouldBe("NotFaked");
            updatedDetails.ClientInfo.Email.ShouldBe("SubEmail5@test.com");
            updatedDetails.ClientInfo.Name.ShouldBe("Name5");
            updatedDetails.ClientInfo.PhoneNumber.ShouldBe("SubPhone5");
            updatedDetails.ClientInfo.CopyPhone.ShouldBe("CopyPhone5");
            updatedDetails.ClientInfo.Department.ShouldBe("Department5");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedValues3()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i+1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            OrderData[1].ClientName.ShouldBeNull();
            OrderData[1].ClientId.ShouldNotBe("NotFaked");
            OrderData[1].AdditionalEmails.ShouldBe("AdditionalEmails2");
            var od = OrderData[1].GetOrderDetails();
            od.Quantity = 0;
            od.SelectedTests = new List<TestDetails>();
            od.SelectedTests.Count.ShouldBe(0);

            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.Quantity.ShouldBe(3);
            updatedDetails.SelectedTests.Count.ShouldBe(5);
            updatedDetails.LabworksSampleDisposition.ShouldBe("toast it");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Fact]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedTotals1()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            var od = OrderData[1].GetOrderDetails();
            od.Quantity = 0;
            od.InternalProcessingFee = 10;
            od.ExternalProcessingFee = 20;
            od.Payment.ClientType = "uc";
            od.SelectedTests = new List<TestDetails>();
            od.SelectedTests.Count.ShouldBe(0);

            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.Quantity.ShouldBe(3);
            updatedDetails.SelectedTests.Count.ShouldBe(5);

            updatedDetails.Total.ShouldBe(26.5m); //(10 + 1.1 + 2.2 + 3.3 + 4.4 + 5.5 ) * 1
            updatedDetails.RushMultiplier.ShouldBe(1);

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Theory]
        [InlineData("uc")]
        [InlineData("UC")]
        [InlineData("Uc")]
        [InlineData("uC")]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedTotals2(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            var od = OrderData[1].GetOrderDetails();
            od.Quantity = 0;
            od.InternalProcessingFee = 10;
            od.ExternalProcessingFee = 20;
            od.Payment.ClientType = value;
            od.SelectedTests = new List<TestDetails>();
            od.SelectedTests.Count.ShouldBe(0);

            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.Quantity.ShouldBe(3);
            updatedDetails.SelectedTests.Count.ShouldBe(5);

            updatedDetails.Total.ShouldBe(39.75m); //(10 + 1.1 + 2.2 + 3.3 + 4.4 + 5.5 ) * 1.5
            updatedDetails.RushMultiplier.ShouldBe(1.5m);

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }


        [Theory]
        [InlineData("x")]
        [InlineData("xx")]
        [InlineData("1")]
        [InlineData(" ")]
        public async Task TestAddRequestNumberPostUpdatesOrderWithExpectedTotals3(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            var od = OrderData[1].GetOrderDetails();
            od.Quantity = 0;
            od.InternalProcessingFee = 10;
            od.ExternalProcessingFee = 20;
            od.Payment.ClientType = value;
            od.SelectedTests = new List<TestDetails>();
            od.SelectedTests.Count.ShouldBe(0);

            OrderData[1].SaveDetails(od);

            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            //Verify Changed
            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.Quantity.ShouldBe(3);
            updatedDetails.SelectedTests.Count.ShouldBe(5);

            updatedDetails.Total.ShouldBe(54.75m); //(20 + 1.1 + 2.2 + 3.3 + 4.4 + 5.5 ) * 1.5
            updatedDetails.RushMultiplier.ShouldBe(1.5m);

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order updated from work request number");
        }

        [Fact]
        public async Task TestAddRequestNumberPostDoesNotModifyLabFinalizeStuff()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].ClientId = null;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);
            //MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(5, true));

            //Verify what it looks like before
            var od = OrderData[1].GetOrderDetails();
            od.Quantity = 0;
            od.InternalProcessingFee = 10;
            od.ExternalProcessingFee = 20;
            od.Payment.ClientType = "x";
            od.SelectedTests = new List<TestDetails>();
            od.SelectedTests.Count.ShouldBe(0);
            od.LabComments = "fake1";
            od.AdjustmentAmount = 1.2m;

            OrderData[1].SaveDetails(od);
            // Act
            var cr = await Controller.AddRequestNumber(OrderData[1].Id, true, "abc");

            // Assert
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("Confirmation");
            rr.ControllerName.ShouldBeNull();
            rr.RouteValues["id"].ShouldBe(2);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("NotFaked"), Times.Once);

            var updatedDetails = OrderData[1].GetOrderDetails();
            updatedDetails.AdjustmentAmount.ShouldBe(1.2m);
            updatedDetails.LabComments.ShouldBe("fake1");
        }
        #endregion AddRequestNumber

        #region Confirmation

        [Fact]
        public async Task TestConfirmationGetReturnsNotFound1()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Confirmation(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestConfirmationGetRedirectsWhenNotConfirmed(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var controllerResult = await Controller.Confirmation(OrderData[1].Id);

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("Orders");
            rdResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only receive a confirmed order");
        }

        [Fact]
        public async Task TestConfirmationGetReturnsView()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;


            // Act
            var controllerResult = await Controller.Confirmation(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderReviewModel>(viewResult.Model);
            modelResult.ShouldNotBeNull();
            modelResult.Order.ShouldNotBeNull();
            modelResult.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderDetails.ShouldNotBeNull();
            modelResult.OrderDetails.ClientInfo.ClientId.ShouldBe("ClientId2");
            modelResult.HideLabDetails.ShouldBeFalse();

            Controller.ErrorMessage.ShouldBeNull();
        }

        [Fact]
        public async Task TestConfirmationPostReturnsNotFound1()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Confirmation(9, new LabReceiveModel());

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);

            MockOrderService.Verify(a => a.SendOrderToAnlab(It.IsAny<Order>()), Times.Never); //This currently does nothing
            MockOrderMessagingService.Verify(a => a.EnqueueReceivedMessage(It.IsAny<Order>(), It.IsAny<bool>()), Times.Never);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestConfirmationPostRedirectsWhenNotConfirmed(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var controllerResult = await Controller.Confirmation(OrderData[1].Id, new LabReceiveModel());

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("Orders");
            rdResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only receive a confirmed order");

            MockOrderService.Verify(a => a.SendOrderToAnlab(It.IsAny<Order>()), Times.Never); //This currently does nothing
            MockOrderMessagingService.Verify(a => a.EnqueueReceivedMessage(It.IsAny<Order>(), It.IsAny<bool>()), Times.Never);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("")]
        public async Task TestConfirmationPostRedirectsWhenNoRequestNumber(string value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].RequestNum = value;

            // Act
            var controllerResult = await Controller.Confirmation(OrderData[1].Id, new LabReceiveModel());

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("AddRequestNumber");
            rdResult.ControllerName.ShouldBeNull();
            rdResult.RouteValues.ShouldNotBeNull();
            rdResult.RouteValues["id"].ShouldBe(OrderData[1].Id);
            Controller.ErrorMessage.ShouldBe("You must add a request number first");

            MockOrderService.Verify(a => a.SendOrderToAnlab(It.IsAny<Order>()), Times.Never); //This currently does nothing
            MockOrderMessagingService.Verify(a => a.EnqueueReceivedMessage(It.IsAny<Order>(), It.IsAny<bool>()), Times.Never);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestConfirmationPostRedirectsWhenSuccessfull(bool value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].RequestNum = "Fake123";
            var od = OrderData[1].GetOrderDetails();
            od.LabComments = null;
            od.AdjustmentAmount = 0;
            OrderData[1].SaveDetails(od);

            var model = new LabReceiveModel();
            model.LabComments = "Lab Updated Jason was here";
            model.AdjustmentAmount = 10.5m;
            model.BypassEmail = value;


            // Act
            var controllerResult = await Controller.Confirmation(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ShouldNotBeNull();
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();
            Controller.Message.ShouldBe("Order marked as received");

            var savedDetails = OrderData[1].GetOrderDetails();
            savedDetails.LabComments.ShouldBe("Lab Updated Jason was here");
            savedDetails.AdjustmentAmount.ShouldBe(10.5m);

            MockOrderService.Verify(a => a.SendOrderToAnlab(OrderData[1]), Times.Once); //This currently does nothing
            MockOrderMessagingService.Verify(a => a.EnqueueReceivedMessage(OrderData[1], value), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion Confirmation

        #region Finalize
        [Fact]
        public async Task TestFinalizeGetReturnsNotFound1()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Finalize(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestFinalizeGetRedirectsWhenNotReceived(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id);

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("Orders");
            rdResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only Complete a Received order");
        }

        [Fact]
        public async Task TestFinalizeGetRedirectsWhenOrderServiceReturnsError1()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            var returnResult = new OverwriteOrderResult();
            returnResult.ErrorMessage = "Fake Error";

            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(returnResult);

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. Error looking up on Labworks: Fake Error");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);

        }

        [Fact]
        public async Task TestFinalizeGetRedirectsWhenOrderServiceReturnsError2()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            var returnResult = new OverwriteOrderResult();
            returnResult.MissingCodes = new List<string>();
            returnResult.MissingCodes.Add("Fake1");
            returnResult.MissingCodes.Add("Fake4");

            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(returnResult);

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. The following codes were not found locally: Fake1,Fake4");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);

        }

        [Fact]
        public async Task TestFinalizeGetReturnsView()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);


            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderReviewModel>(viewResult.Model);
            modelResult.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderDetails.ShouldNotBeNull();
            modelResult.HideLabDetails.ShouldBeFalse();

            Controller.ErrorMessage.ShouldBeNull();
        }





        [Fact]
        public async Task TestFinalizePostReturnsNotFound1()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Finalize(9, new LabFinalizeModel());

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestFinalizePostRedirectsWhenNotReceived(string value)
        {
            // Arrange
            OrderData[1].Status = value;

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, new LabFinalizeModel());

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("Orders");
            rdResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("You can only Complete a Received order");
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TestFinalizePostRedirectsWhenNoFileUploaded()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, new LabFinalizeModel());

            // Assert
            var rdResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            rdResult.ActionName.ShouldBe("Finalize");
            rdResult.ControllerName.ShouldBeNull();
            rdResult.RouteValues["id"].ShouldBe(2);
            Controller.ErrorMessage.ShouldBe("You need to upload the results at this time.");
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TestFinalizePostRedirectsWhenOrderServiceReturnsError1()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            OrderData[1].RequestNum = "Fake123";
            var returnResult = new OverwriteOrderResult();
            returnResult.ErrorMessage = "Fake Error";

            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(returnResult);


            var model = new LabFinalizeModel();
            MockFormFile.Setup(a => a.Length).Returns(100);
            MockFormFile.Setup(a => a.FileName).Returns($"Fake-{OrderData[1].RequestNum}-FileName.pdf");
            model.UploadFile = MockFormFile.Object;

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. Error looking up on Labworks: Fake Error");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task TestFinalizePostRedirectsWhenOrderServiceReturnsError2()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            OrderData[1].RequestNum = "Fake123";
            var returnResult = new OverwriteOrderResult();
            returnResult.MissingCodes = new List<string>();
            returnResult.MissingCodes.Add("Fake1");
            returnResult.MissingCodes.Add("Fake4");

            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(returnResult);

            var model = new LabFinalizeModel();
            MockFormFile.Setup(a => a.Length).Returns(100);
            MockFormFile.Setup(a => a.FileName).Returns($"Fake-{OrderData[1].RequestNum}-FileName.pdf");
            model.UploadFile = MockFormFile.Object;

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Error. Unable to continue. The following codes were not found locally: Fake1,Fake4");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestFinalizePostCallsUploadFileAndSaves(bool value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            OrderData[1].PaymentType = PaymentTypeCodes.Other;
            OrderData[1].RequestNum = "Fake123";
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            var model = new LabFinalizeModel();
            model.BypassEmail = value;
            MockFormFile.Setup(a => a.Length).Returns(100);
            MockFormFile.Setup(a => a.FileName).Returns($"Fake-{OrderData[1].RequestNum}-FileName.pdf");
            model.UploadFile = MockFormFile.Object;
            model.AdjustmentAmount = 10.5m;
            model.LabComments = "These be some comments";


            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");


            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            OrderData[1].Status.ShouldBe(OrderStatusCodes.Finalized);
            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            var savedOrderDetails = OrderData[1].GetOrderDetails();
            savedOrderDetails.LabComments.ShouldBe("These be some comments");
            savedOrderDetails.AdjustmentAmount.ShouldBe(10.5m);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order marked as Finalized");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);
            MockOrderMessagingService.Verify(a => a.EnqueueFinalizedMessage(OrderData[1], value), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(It.IsAny<IFormFile>()), Times.Once);
            MockSlothService.Verify(a => a.MoveMoney(It.IsAny<Order>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestFinalizePostCallsUploadFileAndSavesWhenUcDavisAccount1(bool value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;
            OrderData[1].RequestNum = "Fake123";
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            var model = new LabFinalizeModel();
            model.BypassEmail = value;
            MockFormFile.Setup(a => a.Length).Returns(100);
            MockFormFile.Setup(a => a.FileName).Returns($"Fake-{OrderData[1].RequestNum}-FileName.pdf");
            model.UploadFile = MockFormFile.Object;
            model.AdjustmentAmount = 10.5m;
            model.LabComments = "These be some comments";


            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");

            var slothResponse = new SlothResponseModel();
            slothResponse.Success = false;
            slothResponse.Message = "Fake message";
            MockSlothService.Setup(a => a.MoveMoney(OrderData[1])).ReturnsAsync(slothResponse);

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Finalize");
            redirectResult.ControllerName.ShouldBeNull();

            OrderData[1].Status.ShouldBe(OrderStatusCodes.Finalized);
            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            //var savedOrderDetails = OrderData[1].GetOrderDetails();
            //savedOrderDetails.LabComments.ShouldBe("These be some comments");
            //savedOrderDetails.AdjustmentAmount.ShouldBe(10.5m);

            Controller.ErrorMessage.ShouldBe("There was a problem processing the payment for this account. Fake message");
            Controller.Message.ShouldBeNull();

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);
            MockOrderMessagingService.Verify(a => a.EnqueueFinalizedMessage(OrderData[1], value), Times.Never);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            MockFileStorageService.Verify(a => a.UploadFile(It.IsAny<IFormFile>()), Times.Once);
            MockSlothService.Verify(a => a.MoveMoney(It.IsAny<Order>()), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestFinalizePostCallsUploadFileAndSavesWhenUcDavisAccount2(bool value)
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Received;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;
            OrderData[1].RequestNum = "Fake123";
            var overWriteResult = new OverwriteOrderResult();
            overWriteResult.ClientId = "NotFaked";
            overWriteResult.Quantity = 3;
            overWriteResult.SelectedTests = new List<TestDetails>();
            overWriteResult.RushMultiplier = 1.5m;
            for (int i = 0; i < 5; i++)
            {
                overWriteResult.SelectedTests.Add(CreateValidEntities.TestDetails(i + 1));
            }

            overWriteResult.LabworksSampleDisposition = "toast it";
            MockOrderService.Setup(a => a.OverwriteOrderFromDb(It.IsAny<Order>())).ReturnsAsync(overWriteResult);

            var model = new LabFinalizeModel();
            model.BypassEmail = value;
            MockFormFile.Setup(a => a.Length).Returns(100);
            MockFormFile.Setup(a => a.FileName).Returns($"Fake-{OrderData[1].RequestNum}-FileName.pdf");
            model.UploadFile = MockFormFile.Object;
            model.AdjustmentAmount = 10.5m;
            model.LabComments = "These be some comments";


            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");

            var slothResponse = new SlothResponseModel();
            slothResponse.Success = true;
            slothResponse.KfsTrackingNumber = "FakeKfs";
            slothResponse.Id = SpecificGuid.GetGuid(6);
            MockSlothService.Setup(a => a.MoveMoney(OrderData[1])).ReturnsAsync(slothResponse);

            // Act
            var controllerResult = await Controller.Finalize(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            OrderData[1].Status.ShouldBe(OrderStatusCodes.Finalized);
            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            var savedOrderDetails = OrderData[1].GetOrderDetails();
            savedOrderDetails.LabComments.ShouldBe("These be some comments");
            savedOrderDetails.AdjustmentAmount.ShouldBe(10.5m);

            OrderData[1].KfsTrackingNumber.ShouldBe("FakeKfs");
            OrderData[1].SlothTransactionId.ShouldBe(SpecificGuid.GetGuid(6));
            OrderData[1].Paid.ShouldBeTrue();

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order marked as Finalized and UC Davis account marked as paid");

            MockOrderService.Verify(a => a.OverwriteOrderFromDb(OrderData[1]), Times.Once);
            MockOrderMessagingService.Verify(a => a.EnqueueFinalizedMessage(OrderData[1], value), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(It.IsAny<IFormFile>()), Times.Once);
            MockSlothService.Verify(a => a.MoveMoney(It.IsAny<Order>()), Times.Once);
        }

        #endregion Finalize

        #region OverrideOrder

        [Fact]
        public async Task TestOverrideOrderGetReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.OverrideOrder(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestOverrideOrderGetReturnsView1(bool value)
        {
            // Arrange
            OrderData[1].IsDeleted = value;
            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OverrideOrderModel>(viewResult.Model);

            modelResult.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderReviewModel.OrderDetails.ShouldNotBeNull();
            modelResult.OrderReviewModel.HideLabDetails.ShouldBeFalse();
            modelResult.IsDeleted.ShouldBe(value);
            modelResult.Paid.ShouldBe(OrderData[1].Paid);
            modelResult.Status.ShouldBe(OrderData[1].Status);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestOverrideOrderGetReturnsView2(bool value)
        {
            // Arrange
            OrderData[1].Paid = value;
            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OverrideOrderModel>(viewResult.Model);

            modelResult.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderReviewModel.OrderDetails.ShouldNotBeNull();
            modelResult.OrderReviewModel.HideLabDetails.ShouldBeFalse();
            modelResult.IsDeleted.ShouldBe(OrderData[1].IsDeleted);
            modelResult.Paid.ShouldBe(value);
            modelResult.Status.ShouldBe(OrderData[1].Status);
        }
        [Theory]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestOverrideOrderGetReturnsView3(string value)
        {
            // Arrange
            OrderData[1].Status = value;
            OrderData[1].AdditionalEmails = "1@2.com;test@3.com";
            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OverrideOrderModel>(viewResult.Model);

            modelResult.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            modelResult.OrderReviewModel.OrderDetails.ShouldNotBeNull();
            modelResult.OrderReviewModel.HideLabDetails.ShouldBeFalse();
            modelResult.IsDeleted.ShouldBe(OrderData[1].Paid);
            modelResult.Emails.ShouldBe("1@2.com;test@3.com");
            modelResult.Paid.ShouldBe(OrderData[1].Paid);
            modelResult.Status.ShouldBe(value);
        }


        [Fact]
        public async Task TestOverrideOrderPostReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.OverrideOrder(9, new OverrideOrderModel());

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TestOverrideOrderPostWhenInvalidStatus()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            var model = new OverrideOrderModel();
            model.Status = "Invalid";
            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("OverrideOrder");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);
            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            Controller.ErrorMessage.ShouldBe("Unexpected Status Value: Invalid");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task TestOverrideOrderPostUpdatesPaid(bool value, bool newValue)
        {
            // Arrange
            OrderData[1].Paid = value;
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            var model = new OverrideOrderModel();
            model.Paid = newValue;
            model.Status = OrderData[1].Status;

            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            OrderData[1].ResultsFileIdentifier.ShouldBe("NotChanged");
            OrderData[1].Paid.ShouldBe(newValue);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestOverrideOrderPostUpdatesIsDeleted()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            var model = new OverrideOrderModel();
            model.Status = OrderData[1].Status;
            model.IsDeleted = true;

            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Orders");
            redirectResult.ControllerName.ShouldBeNull();

            OrderData[1].ResultsFileIdentifier.ShouldBe("NotChanged");
            OrderData[1].IsDeleted.ShouldBe(true);

            Controller.ErrorMessage.ShouldBe("Order deleted!!!");
            Controller.Message.ShouldBeNull();

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Received)]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestOverrideOrderPostUpdatesStatus(string value)
        {
            // Arrange
            OrderData[1].Status = "XXX";
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            var model = new OverrideOrderModel();
            model.Status = value;

            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            if (value != OrderStatusCodes.Created)
            {
                redirectResult.ActionName.ShouldBe("Details");
                redirectResult.ControllerName.ShouldBeNull();
                redirectResult.RouteValues["id"].ShouldBe(2);
            }
            else
            {
                redirectResult.ActionName.ShouldBe("Confirmation");
                redirectResult.ControllerName.ShouldBe("Order");
                redirectResult.RouteValues["id"].ShouldBe(2);
            }

            OrderData[1].ResultsFileIdentifier.ShouldBe("NotChanged");
            OrderData[1].Status.ShouldBe(value);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestOverrideOrderPostUpdatesFileUpload()
        {
            // Arrange

            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            var model = new OverrideOrderModel();
            model.Status = OrderData[1].Status;
            model.UploadFile = MockFormFile.Object;
            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");



            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(model.UploadFile), Times.Once);
        }

        [Fact]
        public async Task OverrideOrderPostRedirectsWithErrorWhenEmailsInvalid()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].AdditionalEmails = "Fake";
            var model = new OverrideOrderModel();
            model.Status = OrderData[1].Status;
            model.Emails = "j@j.com;bad@1@com;test2@test.com>";

            model.UploadFile = MockFormFile.Object;
            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");


            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("OverrideOrder");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            Controller.ErrorMessage.ShouldBe("Invalid email detected: bad@1@com");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestOverrideOrderPostClearsEmails(string value)
        {
            // Arrange

            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            OrderData[1].AdditionalEmails = "fake@test.com";
            var model = new OverrideOrderModel();
            model.Emails = value;
            model.Status = OrderData[1].Status;
            model.UploadFile = MockFormFile.Object;
            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");



            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            OrderData[1].AdditionalEmails.ShouldBe(string.Empty);

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(model.UploadFile), Times.Once);
        }

        [Theory]
        [InlineData("t1@t.com")]
        [InlineData("t1@T.com")]
        [InlineData("t1@t.com;TEST@test.com;Bahahah@t.eu")]
        public async Task TestOverrideOrderPostSavesEmails1(string value)
        {
            // Arrange

            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            OrderData[1].AdditionalEmails = "fake@test.com";
            var model = new OverrideOrderModel();
            model.Emails = value;
            model.Status = OrderData[1].Status;
            model.UploadFile = MockFormFile.Object;
            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");



            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            OrderData[1].AdditionalEmails.ShouldBe(value.ToLower());

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(model.UploadFile), Times.Once);
        }

        [Fact]
        public async Task TestOverrideOrderPostSavesEmails2()
        {
            // Arrange

            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].ResultsFileIdentifier = "NotChanged";
            OrderData[1].AdditionalEmails = "fake@test.com";
            var model = new OverrideOrderModel();
            model.Emails = "test1@test.com;dup1@test.com;DUP1@test.com;dup2@TEST.com;dup2@test.COM";
            model.Status = OrderData[1].Status;
            model.UploadFile = MockFormFile.Object;
            MockFileStorageService.Setup(a => a.UploadFile(model.UploadFile)).ReturnsAsync("FakeFileId");



            // Act
            var controllerResult = await Controller.OverrideOrder(OrderData[1].Id, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            OrderData[1].ResultsFileIdentifier.ShouldBe("FakeFileId");
            OrderData[1].AdditionalEmails.ShouldBe("test1@test.com;dup1@test.com;dup2@test.com");

            Controller.ErrorMessage.ShouldBeNull();
            Controller.Message.ShouldBe("Order Updated");

            MockDbContext.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockFileStorageService.Verify(a => a.UploadFile(model.UploadFile), Times.Once);
        }

        #endregion OverrideOrder

        #region Search

        [Fact]
        public void TestSearchGetReturnsView()
        {
            // Arrange

            // Act
            var controllerResult = Controller.Search();

            // Assert
            Assert.IsType<ViewResult>(controllerResult);
        }

        [Fact]
        public async Task TestSearchPostRedirectsWhenNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Search("99");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Search");
            redirectResult.ControllerName.ShouldBeNull();

            Controller.ErrorMessage.ShouldBe("Order Not Found");
        }

        [Theory]
        [InlineData("2", 2)]
        [InlineData("3", 3)]
        public async Task TestTestSearchRedirectsToDetailsWhenFound1(string value, int expectedOrderId)
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Search(value);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(expectedOrderId);

            Controller.ErrorMessage.ShouldBeNull();
        }
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public async Task TestTestSearchRedirectsToDetailsWhenFound2(int value)
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Search(SpecificGuid.GetGuid(value).ToString());

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(value);

            Controller.ErrorMessage.ShouldBeNull();
        }

        [Theory]
        [InlineData("Fake123")]
        [InlineData("Fake124")]
        [InlineData("FAKE124")]
        public async Task TestTestSearchRedirectsToDetailsWhenFound3(string value)
        {
            // Arrange
            OrderData[1].RequestNum = value.ToUpper();

            // Act
            var controllerResult = await Controller.Search(value);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Details");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(2);

            Controller.ErrorMessage.ShouldBeNull();
        }

        #endregion Search

        #region JsonDetails

        [Fact]
        public async Task TestJsonDetailsReturnsNotFound()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.JsonDetails(9);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestJsonDetailsReturnsView()
        {
            // Arrange


            // Act
            var controllerResult = await Controller.JsonDetails(2);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(controllerResult);
            var details = Assert.IsType<OrderDetails>(jsonResult.Value);
            details.ClientInfo.ClientId.ShouldBe("ClientId2");
        }
        #endregion JsonDetails
    }

    [Trait("Category", "Controller Reflection")]
    public class LabControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;
        public LabControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(LabController));
        }
        //protected readonly Type ControllerClass = typeof(LabController);


        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(3);
            authAttribute.ElementAt(0).Roles.ShouldBe($"{RoleCodes.Admin},{RoleCodes.LabUser}");

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>( 3);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(19);
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
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("Orders", 2 + countAdjustment, "Orders-1", showListOfAttributes:true);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Orders", 2 + countAdjustment, "Orders-2", showListOfAttributes: true);

            //2
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Details", 1 + countAdjustment, "Details-1", showListOfAttributes: false);

            //3 & 4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("AddRequestNumber", 1 + countAdjustment, "AddRequestNumberGet-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("AddRequestNumber", 2 + countAdjustment, "AddRequestNumberPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("AddRequestNumber", 2 + countAdjustment, "AddRequestNumberPost - 2", true, showListOfAttributes: false);

            //5 & 6
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmation", 1 + countAdjustment, "ConfirmationGet-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmation", 2 + countAdjustment, "ConfirmationPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Confirmation", 2 + countAdjustment, "ConfirmationPost-2", true, showListOfAttributes: false);

            //7 & 8
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Finalize", 1 + countAdjustment, "FinalizeGet-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Finalize", 2 + countAdjustment, "FinalizePost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Finalize", 2 + countAdjustment, "FinalizePost-2", true, showListOfAttributes: false);

            // 9 & 10
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderGet-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderGet-2", false, showListOfAttributes: false);
            var overrideOrderGetAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderGet-3", false, showListOfAttributes: false);
            overrideOrderGetAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderPost-2", true, showListOfAttributes: false);
            var overrideOrderPostAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("OverrideOrder", 3 + countAdjustment, "OverrideOrderPost-3", true, showListOfAttributes: false);
            overrideOrderPostAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);

            //11
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("JsonDetails", 2 + countAdjustment, "JsonDetails-1", false, showListOfAttributes: false);
            var jsonDetailsAuth = ControllerReflection.MethodExpectedAttribute<AuthorizeAttribute>("JsonDetails", 2 + countAdjustment, "JsonDetails-2", false, showListOfAttributes: false);
            jsonDetailsAuth.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);

            //12 & 13
            ControllerReflection.MethodExpectedNoAttribute("Search", "SearchGet");
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Search", 2 + countAdjustment, "SearchPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Search", 2 + countAdjustment, "SearchPost-2", true, showListOfAttributes: false);

            //14
            ControllerReflection.MethodExpectedAttribute<AllowAnonymousAttribute>("Ping", 2 + countAdjustment, "Ping-1", false, showListOfAttributes: true);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Ping", 2 + countAdjustment, "Ping-2", false, showListOfAttributes: true);

            //#15
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("GeneratePartialResultsEmail", 2 + countAdjustment, "GeneratePartialResultsEmail-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GeneratePartialResultsEmail", 2 + countAdjustment, "GeneratePartialResultsEmail-2", false, showListOfAttributes: false);

            //#16
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("ClearRequestNumber", 2 + countAdjustment, "ClearRequestNumber", false, showListOfAttributes: true);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ClearRequestNumber", 2 + countAdjustment, "ClearRequestNumber", false, showListOfAttributes: true);

            //#17
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("GenerateDisposalEmail", 2 + countAdjustment, "GenerateDisposalEmail-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GenerateDisposalEmail", 2 + countAdjustment, "GenerateDisposalEmail-2", false, showListOfAttributes: false);

            //#18
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("GenerateDisposalEmails", 2 + countAdjustment, "GenerateDisposalEmails-1", false, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GenerateDisposalEmails", 2 + countAdjustment, "GenerateDisposalEmails-2", false, showListOfAttributes: false);

            //#17
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("DisposalList", 1 + countAdjustment, "DisposalList", false, showListOfAttributes: true);
        }


    }
}
