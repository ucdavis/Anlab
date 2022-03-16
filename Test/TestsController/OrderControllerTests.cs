using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
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
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
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
    public class OrderControllerTests
    {
        //Mocks
        public Mock<ApplicationDbContext> MockDbContext { get; set; }
        public Mock<HttpContext> MockHttpContext { get; set; }
        public Mock<IOrderService> MockOrderService { get; set; }
        public Mock<IOrderMessageService> MockOrderMessagingService { get; set; }
        public Mock<ILabworksService> MockLabworksService { get; set; }
        public Mock<IFinancialService> MockFinancialService { get; set; }
        public Mock<IOptions<AppSettings>> MockAppSettings { get; set; }
        public Mock<ClaimsPrincipal> MockClaimsPrincipal { get; set; }
        public Mock<TempDataSerializer> MockTempDataSerializer { get; set; }

        //Setup Data
        public List<Order> OrderData { get; set; }
        public List<TestItemModel> TestItemModelData { get; set; }
        public List<User> UserData { get; set; }



        //Controller
        public OrderController Controller { get; set; }

        /// <summary>
        /// Test Setup
        /// </summary>
        public OrderControllerTests()
        {
            //To return the user so can check identity.
            MockHttpContext = new Mock<HttpContext>();

            MockOrderService = new Mock<IOrderService>();
            MockOrderMessagingService = new Mock<IOrderMessageService>();
            MockLabworksService = new Mock<ILabworksService>();
            MockFinancialService = new Mock<IFinancialService>();
            MockAppSettings = new Mock<IOptions<AppSettings>>();
            MockDbContext = new Mock<ApplicationDbContext>();
            MockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            MockTempDataSerializer = new Mock<TempDataSerializer>();
            var mockDataProvider = new Mock<SessionStateTempDataProvider>(MockTempDataSerializer.Object);

            //Default data
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Creator1"),
            }));

            TestItemModelData = new List<TestItemModel>();
            for (int i = 0; i < 10; i++)
            {
                TestItemModelData.Add(CreateValidEntities.TestItemModel(i + 1));
            }

            OrderData = new List<Order>();
            for (int i = 0; i < 3; i++)
            {
                var order = CreateValidEntities.Order(i + 1, true);
                order.Creator = CreateValidEntities.User(2);
                OrderData.Add(order);
            }

            var proc = new TestItemPrices();
            proc.Id = "PROC";
            proc.InternalCost = 6m;

            var appSettings = new AppSettings();
            appSettings.NonUcRate = 1.9m;

            UserData = new List<User>()
            {
                CreateValidEntities.User(1, true),
                CreateValidEntities.User(2, true)
            };
            UserData[0].Id = "Creator1";

            //Setups
            MockClaimsPrincipal.Setup(a => a.Claims).Returns(user.Claims);
            MockClaimsPrincipal.Setup(a => a.IsInRole(RoleCodes.Admin)).Returns(false);
            MockClaimsPrincipal.Setup(a => a.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, "Creator1"));

            MockHttpContext.Setup(m => m.User).Returns(MockClaimsPrincipal.Object);



            MockDbContext.Setup(m => m.Orders).Returns(OrderData.AsQueryable().MockAsyncDbSet().Object);
            MockDbContext.Setup(a => a.Users).Returns(UserData.AsQueryable().MockAsyncDbSet().Object);
            MockOrderService.Setup(a => a.PopulateTestItemModel(It.IsAny<bool>())).ReturnsAsync(TestItemModelData);
            MockOrderService.Setup(a => a.PopulateOrder(It.IsAny<OrderSaveModel>(), It.IsAny<Order>()));
            MockLabworksService.Setup(a => a.GetPrice("PROC")).ReturnsAsync(proc);
            MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(3));
            MockAppSettings.Setup(a => a.Value).Returns(appSettings);

            //The controller
            Controller = new OrderController(MockDbContext.Object,
                MockOrderService.Object,
                MockOrderMessagingService.Object,
                MockLabworksService.Object,
                MockFinancialService.Object,
                MockAppSettings.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = MockHttpContext.Object
                },
                TempData = new TempDataDictionary(MockHttpContext.Object, mockDataProvider.Object)
            };
        }
        [Fact]
        public async Task OrderIndexReturnsView()
        {
            //Arrange
            OrderData[2].CreatorId = "Creator1";
            OrderData[0].CreatorId = "Creator1";

            //Act
            var controllerResult = await Controller.Index();

            //Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<OrderListModel[]>(result.Model);
            model.Length.ShouldBe(2);
            model[0].Id.ShouldBe(1);
            model[1].Id.ShouldBe(3);
        }




        [Fact]
        public async Task CreateCallsOrderService()
        {
            var controllerResult = await Controller.Create();
            MockOrderService.Verify(a => a.PopulateTestItemModel(It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task CreateCallsLabworksServiceGetPrice()
        {
            var controllerResult = await Controller.Create();
            MockLabworksService.Verify(a => a.GetPrice("PROC"), Times.Once);
        }

        [Fact]
        public async Task CreateDoesNotCallLabworksGetClientDetails()
        {
            UserData[0].ClientId = null;
            var controllerResult = await Controller.Create();
            MockLabworksService.Verify(a => a.GetClientDetails(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateDoesCallLabworksGetClientDetails()
        {
            UserData[0].ClientId = "12345";
            var controllerResult = await Controller.Create();
            MockLabworksService.Verify(a => a.GetClientDetails("12345"), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateReturnsViewWithExpectedData1()
        {
            UserData[0].ClientId = "12345";
            var controllerResult = await Controller.Create();

            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<OrderEditModel>(result.Model);
            model.TestItems.Length.ShouldBe(10);
            model.InternalProcessingFee.ShouldBe(6m);
            model.ExternalProcessingFee.ShouldBe(12m);
            var defaults = Assert.IsType<OrderEditDefaults>(model.Defaults);
            defaults.DefaultAccount.ShouldBe("ACCOUNT1"); //Uppercase
            defaults.DefaultEmail.ShouldBe("test1@test.com");
            defaults.DefaultCompanyName.ShouldBe("CompanyName1");
            defaults.DefaultAcAddr.ShouldBe("BillingContactAddress1");
            defaults.DefaultAcEmail.ShouldBe("BillingContactEmail1@test.com");
            defaults.DefaultAcName.ShouldBe("BillingContactName1");
            defaults.DefaultAcPhone.ShouldBe("BillingContactPhone1");

            defaults.DefaultClientId.ShouldBe("ClientId3"); //In reality, this would be the same as the user client id.
            defaults.DefaultClientIdName.ShouldBe("Name3");
            defaults.DefaultSubEmail.ShouldBe("SubEmail3@test.com");
            defaults.DefaultCopyEmail.ShouldBe("CopyEmail3@test.com");
        }

        [Fact]
        public async Task CreateReturnsViewWithExpectedData2()
        {
            UserData[0].ClientId = "12345";
            UserData[0].Account = null; //Default account will be from labworks

            var controllerResult = await Controller.Create();

            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<OrderEditModel>(result.Model);
            var defaults = Assert.IsType<OrderEditDefaults>(model.Defaults);
            defaults.DefaultAccount.ShouldBe("DefaultAccount3");
        }

        [Fact]
        public async Task EditReturnsNotFoundWhenOrderIdNotFound()
        {
            var controllerResult = await Controller.Edit(99);
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestEditRedirectsToIndexWhenNotCreatedByCurrentUser()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null; //Clear just in case

            // Act

            var controllerResult = await Controller.Edit(2);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestEditRedirectsToIndexWhenNotNotInCreatedStatus()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].CreatorId = "Creator1";
            Controller.ErrorMessage = null; //Clear just in case

            // Act

            var controllerResult = await Controller.Edit(2);
            Controller.ErrorMessage.ShouldBe("You can\'t edit an order that has been confirmed.");
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestEditReturnsViewWithExpectedResults()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].Creator = CreateValidEntities.User(7);

            // Act
            var controllerResult = await Controller.Edit(2);

            // Assert
            MockOrderService.Verify(a => a.PopulateTestItemModel(It.IsAny<bool>()), Times.Once);
            MockLabworksService.Verify(a => a.GetPrice("PROC"), Times.Once);

            var result = Assert.IsType<ViewResult>(controllerResult);
            var model = Assert.IsType<OrderEditModel>(result.Model);
            model.Order.ShouldNotBeNull();
            model.Order.Id.ShouldBe(2);
            model.TestItems.Length.ShouldBe(10);
            model.InternalProcessingFee.ShouldBe(6m);
            model.ExternalProcessingFee.ShouldBe(12m);
            model.Defaults.DefaultEmail.ShouldBe("test7@testy.com");
        }

        [Fact]
        public async Task TestCopyReturnsNotFoundIfOrderNotFound()
        {
            // Arrange



            // Act
            var controllerResult = await Controller.Copy(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestCopySetsTheCreatorToCurrectUserWhenNotAdmin()
        {
            // Arrange
            Order savedResult = null;
            MockDbContext.Setup(a => a.Add(It.IsAny<Order>())).Callback<Order>(r => savedResult = r);


            var copiedOrder = CreateValidEntities.Order(7);
            copiedOrder.Creator = CreateValidEntities.User(5, true);
            OrderData[1].CreatorId = "xxx";
            OrderData[1].Creator = CreateValidEntities.User(5);
            MockOrderService.Setup(a => a.DuplicateOrder(OrderData[1], false)).ReturnsAsync(copiedOrder);
            // Act
            var controllerResult = await Controller.Copy(SpecificGuid.GetGuid(2));

            // Assert
            MockOrderService.Verify(a => a.DuplicateOrder(OrderData[1], false), Times.Once);
            MockDbContext.Verify(a => a.Add(It.IsAny<Order>()), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);

            savedResult.ShouldNotBeNull();
            savedResult.CreatorId.ShouldBe("Creator1");
            savedResult.Creator.Id.ShouldBe("Creator1");
            savedResult.ShareIdentifier.ShouldNotBe(SpecificGuid.GetGuid(2));
            savedResult.SavedTestDetails.ShouldBeNull();

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Edit");
            redirectResult.ControllerName.ShouldBeNull();
            redirectResult.RouteValues["id"].ShouldBe(savedResult.Id);
        }

        [Fact]
        public void TestCopySetsTheCreatorToCurrectUserWhenAdminIsTrueButNotAdminRole()
        {
            // Arrange
            Order savedResult = null;
            MockDbContext.Setup(a => a.Add(It.IsAny<Order>())).Callback<Order>(r => savedResult = r);


            var copiedOrder = CreateValidEntities.Order(7);
            copiedOrder.Creator = CreateValidEntities.User(5, true);
            OrderData[1].CreatorId = "xxx";
            OrderData[1].Creator = CreateValidEntities.User(5);
            MockOrderService.Setup(a => a.DuplicateOrder(OrderData[1], true)).ReturnsAsync(copiedOrder);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await Controller.Copy(SpecificGuid.GetGuid(2), true));

            // Assert

            ex.Result.Message.ShouldBe("Permissions Missing");


            MockOrderService.Verify(a => a.DuplicateOrder(OrderData[1], true), Times.Once);
            MockDbContext.Verify(a => a.Add(It.IsAny<Order>()), Times.Never);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestCopySetsTheCreatorToCorrectUserWhenAdminIsTrue()
        {
            MockClaimsPrincipal.Setup(a => a.IsInRole(RoleCodes.Admin)).Returns(true);

            Order savedResult = null;
            MockDbContext.Setup(a => a.Add(It.IsAny<Order>())).Callback<Order>(r => savedResult = r);


            var copiedOrder = CreateValidEntities.Order(7, true);
            var testItemModel = new List<TestItemModel>();
            for (int i = 0; i < 5; i++)
            {
                testItemModel.Add(CreateValidEntities.TestItemModel(1));
            }
            copiedOrder.SaveTestDetails(testItemModel);
            OrderData[1].SaveTestDetails(testItemModel);

            copiedOrder.Creator = CreateValidEntities.User(5, true);
            OrderData[1].CreatorId = "xxx";
            OrderData[1].Creator = CreateValidEntities.User(5);
            MockOrderService.Setup(a => a.DuplicateOrder(OrderData[1], true)).ReturnsAsync(copiedOrder);
            // Act
            var controllerResult = await Controller.Copy(SpecificGuid.GetGuid(2), true);

            // Assert
            MockOrderService.Verify(a => a.DuplicateOrder(OrderData[1], true), Times.Once);
            MockDbContext.Verify(a => a.Add(It.IsAny<Order>()), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);

            savedResult.ShouldNotBeNull();
            savedResult.CreatorId.ShouldBe("xxx");
            savedResult.Creator.Id.ShouldBe("5");
            savedResult.ShareIdentifier.ShouldNotBe(SpecificGuid.GetGuid(2));
            savedResult.Status.ShouldBe(OrderStatusCodes.Confirmed);
            savedResult.GetOrderDetails().LabComments.ShouldContain("Duplicated from 2");
            savedResult.GetTestDetails().Count.ShouldBe(5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("AddRequestNumber");
            redirectResult.ControllerName.ShouldBe("Lab");
            redirectResult.RouteValues["id"].ShouldBe(savedResult.Id);

            MockClaimsPrincipal.Verify(a => a.IsInRole(RoleCodes.Admin), Times.Once);
        }

        [Fact]
        public async Task TestSaveReturnsJsonWhenModelStateInvalid()
        {
            // Arrange
            Controller.ModelState.AddModelError("Fake", "FakeError");


            // Act
            var controllerResult = await Controller.Save(null);

            // Assert
            var result = Assert.IsType<JsonResult>(controllerResult);
            dynamic data = JObject.FromObject(result.Value);
            ((string)data.message).ShouldBe("There were problems with your order. Unable to save. Errors: FakeError");
            ((bool)data.success).ShouldBe(false);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestSaveChecksIfItIsYourOrder()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            var model = new OrderSaveModel();
            model.OrderId = 2;


            // Act
            var controllerResult = await Controller.Save(model);

            // Assert
            var result = Assert.IsType<JsonResult>(controllerResult);
            dynamic data = JObject.FromObject(result.Value);
            ((string)data.message).ShouldBe("This is not your order.");
            ((bool)data.success).ShouldBe(false);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestSaveChecksIfYourOrderIsInCreatedStatus()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            OrderData[1].CreatorId = "Creator1";
            var model = new OrderSaveModel();
            model.OrderId = 2;


            // Act
            var controllerResult = await Controller.Save(model);

            // Assert
            var result = Assert.IsType<JsonResult>(controllerResult);
            dynamic data = JObject.FromObject(result.Value);
            ((string)data.message).ShouldBe("This has been confirmed and may not be updated.");
            ((bool)data.success).ShouldBe(false);

            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestSaveWhenEditAndSuccess()
        {
            // Arrange
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].CreatorId = "Creator1";
            var model = new OrderSaveModel();
            model.OrderId = 2;

            OrderData[1].SavedTestDetails.ShouldBeNull();

            // Act
            var controllerResult = await Controller.Save(model);

            // Assert
            var result = Assert.IsType<JsonResult>(controllerResult);
            dynamic data = JObject.FromObject(result.Value);
            ((bool)data.success).ShouldBe(true);
            ((int)data.id).ShouldBe(2);

            MockOrderService.Verify(a => a.PopulateTestItemModel(It.IsAny<bool>()), Times.Once);
            MockOrderService.Verify(a => a.PopulateOrder(model, OrderData[1]), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            OrderData[1].SavedTestDetails.ShouldNotBeNull();
            OrderData[1].GetTestDetails().Count.ShouldBe(10);
        }

        [Fact]
        public async Task TestSaveWhenCreateAndSuccess()
        {
            // Arrange
            var model = new OrderSaveModel();
            model.OrderId = null;

            Order savedResult = null;
            MockDbContext.Setup(a => a.Add(It.IsAny<Order>())).Callback<Order>(r => savedResult = r);

            // Act
            var controllerResult = await Controller.Save(model);

            // Assert
            var result = Assert.IsType<JsonResult>(controllerResult);
            dynamic data = JObject.FromObject(result.Value);
            ((bool)data.success).ShouldBe(true);
            ((int)data.id).ShouldBe(0); //Because it would be set in the DB

            MockOrderService.Verify(a => a.PopulateTestItemModel(It.IsAny<bool>()), Times.Once);
            MockOrderService.Verify(a => a.PopulateOrder(model, It.IsAny<Order>()), Times.Once);
            MockDbContext.Verify(a => a.Add(It.IsAny<Order>()), Times.Once);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);

            savedResult.CreatorId.ShouldBe("Creator1");
            savedResult.Creator.ShouldNotBeNull();
            savedResult.Creator.Id.ShouldBe("Creator1");
            savedResult.Status.ShouldBe(OrderStatusCodes.Created);
            savedResult.ShareIdentifier.ShouldNotBe(default);
        }

        [Fact]
        public async Task TestDetailsReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Details(99);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestDetailsReturnsNotFoundWhenYouDoNotHaveAccess()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Details(2);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
        }

        [Fact]
        public async Task TestDetailsRedirectsWhenInCreatedStatus()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Details(2);

            // Assert
            var redirectRoute = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectRoute.ActionName.ShouldBe("Index");
            redirectRoute.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("Must confirm order before viewing details.");
        }

        [Fact]
        public async Task TestDetailsReturnsView()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Quantity = 3;
            OrderData[1].SaveDetails(orderDetails);

            // Act
            var controllerResult = await Controller.Details(2);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderReviewModel>(result.Model);
            resultModel.Order.ShouldBe(OrderData[1]);
            resultModel.OrderDetails.Quantity.ShouldBe(3);
        }

        [Fact]
        public async Task TestConfirmationGetReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Confirmation(99);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestConfirmationGetReturnsNotFoundWhenYouDoNotHaveAccess()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Confirmation(2);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
        }

        [Fact]
        public async Task TestConfirmationGetReturnsView()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Quantity = 5;
            OrderData[1].SaveDetails(orderDetails);

            // Act
            var controllerResult = await Controller.Confirmation(2);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderReviewModel>(result.Model);
            resultModel.Order.ShouldBe(OrderData[1]);
            resultModel.OrderDetails.Quantity.ShouldBe(5);

            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);
        }

        [Fact]
        public async Task TestConfirmationPostReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Confirmation(99, true);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestConfirmationPostReturnsNotFoundWhenYouDoNotHaveAccess()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestConfirmationPostRedirectsWhenAlreadyConfirmed()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("Already confirmed");
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task TestConfirmationPostWhenUcdAccountNotVerified1()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Payment.Account = "3-1234567";
            orderDetails.Payment.AccountName = "WHAT!";
            OrderData[1].SaveDetails(orderDetails);

            MockFinancialService.Setup(a => a.GetAccountName(It.IsAny<string>())).Throws(new Exception("Fake"));

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBe("Unable to verify UC Account number. Please edit your order and re-enter the UC account number. Then try again.");
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBeEmpty();
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName("3-1234567"), Times.Once);
        }

        [Fact]
        public async Task TestConfirmationPostWhenUcdAccountNotVerified2()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Payment.Account = "3-1234567";
            orderDetails.Payment.AccountName = "WHAT!";
            OrderData[1].SaveDetails(orderDetails);

            MockFinancialService.Setup(a => a.GetAccountName(It.IsAny<string>())).ReturnsAsync(string.Empty);

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBe("Unable to verify UC Account number. Please edit your order and re-enter the UC account number. Then try again.");
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBeEmpty();
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName("3-1234567"), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Confirmation");
            redirectResult.RouteValues["id"].ShouldBe(2);
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestConfirmationPostWhenUcdAccountVerified()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Payment.Account = "3-1234567";
            orderDetails.Payment.AccountName = "WHAT!";
            orderDetails.ClientInfo.ClientId = null;
            OrderData[1].SaveDetails(orderDetails);

            MockFinancialService.Setup(a => a.GetAccountName(It.IsAny<string>())).ReturnsAsync("My Fake Account");

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBeNull();
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBe("My Fake Account");
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName("3-1234567"), Times.Once);
            MockOrderService.Verify(a => a.UpdateAdditionalInfo(OrderData[1]), Times.Once);
            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Confirmed");
            redirectResult.RouteValues["id"].ShouldBe(2);
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestConfirmationPostWhenPaymentOther1()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.Other;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.OtherPaymentInfo.PaymentType = "AGReemeNT";
            orderDetails.ClientInfo.ClientId = null;
            OrderData[1].SaveDetails(orderDetails);

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBeNull();
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBeNull();
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName(It.IsAny<string>()), Times.Never);
            MockOrderMessagingService.Verify(a => a.EnqueueBillingMessage(OrderData[1], "Anlab Work Order Billing Info -- Agreement"), Times.Once);
            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);
            MockOrderService.Verify(a => a.UpdateAdditionalInfo(OrderData[1]), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails(It.IsAny<string>()), Times.Never);
            MockOrderMessagingService.Verify(a => a.EnqueueCreatedMessage(OrderData[1]), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Confirmed");
            redirectResult.RouteValues["id"].ShouldBe(2);
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestConfirmationPostWhenPaymentOther2()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.Other;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.OtherPaymentInfo.PaymentType = "SomethingElse";
            orderDetails.ClientInfo.ClientId = null;
            OrderData[1].SaveDetails(orderDetails);

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBeNull();
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBeNull();
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName(It.IsAny<string>()), Times.Never);
            MockOrderMessagingService.Verify(a => a.EnqueueBillingMessage(OrderData[1], It.IsAny<string>()), Times.Never);
            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);
            MockOrderService.Verify(a => a.UpdateAdditionalInfo(OrderData[1]), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails(It.IsAny<string>()), Times.Never);
            MockOrderMessagingService.Verify(a => a.EnqueueCreatedMessage(OrderData[1]), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Confirmed");
            redirectResult.RouteValues["id"].ShouldBe(2);
            redirectResult.ControllerName.ShouldBeNull();
        }

        [Fact]
        public async Task TestConfirmationPostWhenClientIdHasValue()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            OrderData[1].PaymentType = PaymentTypeCodes.Other;
            Controller.ErrorMessage = null;


            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.OtherPaymentInfo.PaymentType = "SomethingElse";
            orderDetails.ClientInfo.ClientId = "FAKE1";
            OrderData[1].SaveDetails(orderDetails);

            MockLabworksService.Setup(a => a.GetClientDetails(It.IsAny<string>())).ReturnsAsync(CreateValidEntities.ClientDetailsLookupModel(2));

            // Act
            var controllerResult = await Controller.Confirmation(2, true);

            // Assert
            Controller.ErrorMessage.ShouldBeNull();
            OrderData[1].GetOrderDetails().Payment.AccountName.ShouldBeNull();
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockFinancialService.Verify(a => a.GetAccountName(It.IsAny<string>()), Times.Never);
            MockOrderMessagingService.Verify(a => a.EnqueueBillingMessage(OrderData[1], It.IsAny<string>()), Times.Never);
            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);
            MockOrderService.Verify(a => a.UpdateAdditionalInfo(OrderData[1]), Times.Once);
            MockLabworksService.Verify(a => a.GetClientDetails("FAKE1"), Times.Once);
            MockOrderMessagingService.Verify(a => a.EnqueueCreatedMessage(OrderData[1]), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Confirmed");
            redirectResult.RouteValues["id"].ShouldBe(2);
            redirectResult.ControllerName.ShouldBeNull();

            var od = OrderData[1].GetOrderDetails();
            od.ClientInfo.Email.ShouldBe("SubEmail2@test.com");
            od.ClientInfo.PhoneNumber.ShouldBe("SubPhone2");
            od.ClientInfo.CopyPhone.ShouldBe("CopyPhone2");
            od.ClientInfo.Department.ShouldBe("Department2");

        }

        [Fact]
        public async Task TestConfirmedReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Confirmed(99);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestConfirmedReturnsNotFoundWhenYouDoNotHaveAccess()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Confirmed(2);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
        }

        [Fact]
        public async Task TestConfirmedReturnsView()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            var orderDetails = CreateValidEntities.OrderDetails(2);
            orderDetails.Quantity = 5;
            OrderData[1].SaveDetails(orderDetails);

            // Act
            var controllerResult = await Controller.Confirmation(2);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderReviewModel>(result.Model);
            resultModel.Order.ShouldBe(OrderData[1]);
            resultModel.OrderDetails.Quantity.ShouldBe(5);

            MockOrderService.Verify(a => a.UpdateTestsAndPrices(OrderData[1]), Times.Once);
        }

        [Fact]
        public async Task TestDeleteReturnsNotFound()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            MockDbContext.Verify(a => a.Remove(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task TestDeleteReturnsNotFoundWhenYouDoNotHaveAccess()
        {
            // Arrange
            OrderData[1].CreatorId = "XXX";
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Delete(2);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
            Controller.ErrorMessage.ShouldBe("You don't have access to this order.");
            MockDbContext.Verify(a => a.Remove(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task TestDeleteRedirectsWhenAlreadyConfirmed()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Confirmed;
            Controller.ErrorMessage = null;

            // Act
            var controllerResult = await Controller.Delete(2);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();
            Controller.ErrorMessage.ShouldBe("Can't delete confirmed orders.");
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Never);
            MockDbContext.Verify(a => a.Remove(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task TestDeleteRedirectsAndDeletes()
        {
            // Arrange
            OrderData[1].CreatorId = "Creator1";
            OrderData[1].Status = OrderStatusCodes.Created;
            Controller.Message = null;

            // Act
            var controllerResult = await Controller.Delete(2);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(controllerResult);
            redirectResult.ActionName.ShouldBe("Index");
            redirectResult.ControllerName.ShouldBeNull();
            Controller.Message.ShouldBe("Order deleted");
            MockDbContext.Verify(a => a.SaveChangesAsync(new CancellationToken()), Times.Once);
            MockDbContext.Verify(a => a.Remove(OrderData[1]), Times.Once);
        }


        [Fact]
        public async Task TestLookupClinetIdReturnsExpectedValues()
        {
            // Arrange


            // Act
            var controllerResult = await Controller.LookupClientId("Test");

            // Assert
            var result = Assert.IsType<ClientDetailsLookupModel>(controllerResult);
            result.Name.ShouldBe("Name3");

            MockLabworksService.Verify(a => a.GetClientDetails("Test"), Times.Once);

        }
    }

    [Trait("Category", "Controller Reflection")]
    public class OrderControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public OrderControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(OrderController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(3);
            authAttribute.ElementAt(0).Roles.ShouldBeNull();

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(3);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(14);
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
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Index", 1 + countAdjustment, "Index-1", showListOfAttributes: false);

            //2
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Create", 1 + countAdjustment, "Create-1", showListOfAttributes: false);

            //3
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Edit", 1 + countAdjustment, "Edit-1", showListOfAttributes: false);

            //4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Copy", 1 + countAdjustment, "Copy-1", showListOfAttributes: false);

            //5
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Save", 2 + countAdjustment, "Save-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Save", 2 + countAdjustment, "Save-2", showListOfAttributes: false);

            //6
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Details", 1 + countAdjustment, "Details-1", showListOfAttributes: false);

            //7 & 8
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmation", 1 + countAdjustment, "CopyGet-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmation", 2 + countAdjustment, "CopyPost-1", true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Confirmation", 2 + countAdjustment, "CopyPost-2", true, showListOfAttributes: false);

            //9
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmed", 1 + countAdjustment, "Confirmed-1", showListOfAttributes: false);

            //10
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Delete", 2 + countAdjustment, "Delete-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Delete", 2 + countAdjustment, "Delete-2", showListOfAttributes: false);

            //11
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("LookupClientId", 2 + countAdjustment, "LookupClientId-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("LookupClientId", 2 + countAdjustment, "LookupClientId-2", showListOfAttributes: false);

            //12
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Favorites", 1 + countAdjustment, "Favorites-1", showListOfAttributes: false);

            //13
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("SaveLink", 2 + countAdjustment, "SaveLink-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("SaveLink", 2 + countAdjustment, "SaveLink-1", showListOfAttributes: false);

            //14
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("DeleteLink", 2 + countAdjustment, "DeleteLink-1", showListOfAttributes: true);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("DeleteLink", 2 + countAdjustment, "DeleteLink-1", showListOfAttributes: false);

        }

    }



}
