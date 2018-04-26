using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Configuration;
using AnlabMvc.Models.FileUploadModels;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class ResultsControllerTests
    {
        //Mocks
        public Mock<ApplicationDbContext> MockDbContext { get; set; }
        public Mock<HttpContext> MockHttpContext { get; set; } //Probably don't need this
        public Mock<IFileStorageService> MockFileStorageService { get; set; }
        public Mock<IDataSigningService> MockDataSigningService { get; set; }
        public Mock<IOptions<CyberSourceSettings>> MockCyberSourceSettings { get; set; }
        public Mock<IOptions<AppSettings>> MockAppSettings { get; set; }
        public Mock<IOrderMessageService> MockOrderMessageService { get; set; }

        //Setup Data
        public List<Order> OrderData { get; set; }
        public CyberSourceSettings CyberSourceSettings { get; set; }
        public AppSettings AppSettings { get; set; }

        //Controller
        public ResultsController Controller { get; set; }
        public ResultsControllerTests()
        {
            MockDbContext = new Mock<ApplicationDbContext>();
            MockHttpContext = new Mock<HttpContext>();
            MockFileStorageService = new Mock<IFileStorageService>();
            MockDataSigningService = new Mock<IDataSigningService>();
            MockCyberSourceSettings = new Mock<IOptions<CyberSourceSettings>>();
            MockAppSettings = new Mock<IOptions<AppSettings>>();
            MockOrderMessageService = new Mock<IOrderMessageService>();
            var mockDataProvider = new Mock<SessionStateTempDataProvider>();


            //Data
            OrderData = new List<Order>();
            for (int i = 0; i < 3; i++)
            {
                var order = CreateValidEntities.Order(i + 1, true);
                order.Creator = CreateValidEntities.User(2);
                OrderData.Add(order);
            }
            CyberSourceSettings = new CyberSourceSettings();
            CyberSourceSettings.AccessKey = "123";
            CyberSourceSettings.ProfileId = "myProfile";
            
            AppSettings = new AppSettings();
            AppSettings.CyberSourceUrl = "Http://FakeUrl.com";

            //Setup
            MockDbContext.Setup(a => a.Orders).Returns(OrderData.AsQueryable().MockAsyncDbSet().Object);
            MockCyberSourceSettings.Setup(a => a.Value).Returns(CyberSourceSettings);
            MockAppSettings.Setup(a => a.Value).Returns(AppSettings);

            Controller = new ResultsController(MockDbContext.Object,
                MockFileStorageService.Object,
                MockDataSigningService.Object,
                MockCyberSourceSettings.Object,
                MockAppSettings.Object,
                MockOrderMessageService.Object)
            {
                TempData = new TempDataDictionary(MockHttpContext.Object, mockDataProvider.Object)
            };
        }

        [Fact]
        public async Task TestLinkReturnsNotFound()
        {
            // Arrange
            
            // Act
            var controllerResult = await Controller.Link(SpecificGuid.GetGuid(15));
            
            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Created)]
        [InlineData(OrderStatusCodes.Confirmed)]
        [InlineData(OrderStatusCodes.Received)]
        public async Task TestLinkReturnsNotFoundWhenStatusInvalid(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestLinkReturnsViewWhenOther(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            OrderData[1].PaymentType = PaymentTypeCodes.Other;


            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderResultsModel>(result.Model);
            resultModel.OrderReviewModel.Order.ShouldNotBeNull();
            resultModel.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            resultModel.OrderReviewModel.Order.ShareIdentifier.ShouldBe(OrderData[1].ShareIdentifier);
            resultModel.ShowCreditCardPayment.ShouldBe(false);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestLinkReturnsViewWhenUcDavisAccount(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            OrderData[1].PaymentType = PaymentTypeCodes.UcDavisAccount;


            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderResultsModel>(result.Model);
            resultModel.OrderReviewModel.Order.ShouldNotBeNull();
            resultModel.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            resultModel.OrderReviewModel.Order.ShareIdentifier.ShouldBe(OrderData[1].ShareIdentifier);
            resultModel.ShowCreditCardPayment.ShouldBe(false);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestLinkReturnsViewWhenUcOtherAccount(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            OrderData[1].PaymentType = PaymentTypeCodes.UcOtherAccount;


            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderResultsModel>(result.Model);
            resultModel.OrderReviewModel.Order.ShouldNotBeNull();
            resultModel.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            resultModel.OrderReviewModel.Order.ShareIdentifier.ShouldBe(OrderData[1].ShareIdentifier);
            resultModel.ShowCreditCardPayment.ShouldBe(false);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestLinkReturnsViewWhenCreditCardAlreadyPaid(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = true;


            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderResultsModel>(result.Model);
            resultModel.OrderReviewModel.Order.ShouldNotBeNull();
            resultModel.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            resultModel.OrderReviewModel.Order.ShareIdentifier.ShouldBe(OrderData[1].ShareIdentifier);
            resultModel.ShowCreditCardPayment.ShouldBe(false);
        }

        [Theory]
        [InlineData(OrderStatusCodes.Finalized)]
        [InlineData(OrderStatusCodes.Complete)]
        public async Task TestLinkReturnsViewWhenCreditCardNotPaid(string status)
        {
            // Arrange
            OrderData[1].Status = status;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;


            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<OrderResultsModel>(result.Model);
            resultModel.OrderReviewModel.Order.ShouldNotBeNull();
            resultModel.OrderReviewModel.Order.Id.ShouldBe(OrderData[1].Id);
            resultModel.OrderReviewModel.Order.ShareIdentifier.ShouldBe(OrderData[1].ShareIdentifier);
            resultModel.ShowCreditCardPayment.ShouldBe(true);
        }

        [Fact]
        public async Task TestLinkCallsSetDictWhenUnpaidCreditCard()
        {
            // Arrange
            Dictionary<string, string> dictionary = null;
            MockDataSigningService.Setup(a => a.Sign(It.IsAny<Dictionary<string, string>>())).Returns("FakeSign").Callback<Dictionary<string, string>>(c => dictionary = c);
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            Assert.IsType<ViewResult>(controllerResult);
            MockDataSigningService.Verify(a => a.Sign(It.IsAny<Dictionary<string, string>>()),Times.Once);
            dictionary.ShouldNotBeNull();
        }

        [Fact]
        public async Task TestLinkCallsSetsDictToExpectedValuesWhenUnpaidCreditCard()
        {
            // Arrange
            Dictionary<string, string> dictionary = null;
            MockDataSigningService.Setup(a => a.Sign(It.IsAny<Dictionary<string, string>>())).Returns("FakeSign").Callback<Dictionary<string, string>>(c => dictionary = c);
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;
            var od = OrderData[1].GetOrderDetails();
            od.AdjustmentAmount = 2.51m;
            OrderData[1].SaveDetails(od);

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            Assert.IsType<ViewResult>(controllerResult);
            MockDataSigningService.Verify(a => a.Sign(It.IsAny<Dictionary<string, string>>()), Times.Once);
            dictionary.ShouldNotBeNull();
            dictionary.Count.ShouldBe(16);
            dictionary["transaction_type"].ShouldBe("sale");
            dictionary["reference_number"].ShouldBe("2");
            dictionary["amount"].ShouldBe("2.51");
            dictionary["currency"].ShouldBe("USD");
            dictionary["access_key"].ShouldBe("123");
            dictionary["profile_id"].ShouldBe("myProfile");
            dictionary["transaction_uuid"].ShouldNotBeNull();
            dictionary["signed_date_time"].ShouldNotBeNull();
            dictionary["unsigned_field_names"].ShouldBe(string.Empty);
            dictionary["locale"].ShouldBe("en");
            dictionary["bill_to_email"].ShouldBe("test2@testy.com");
            dictionary["bill_to_forename"].ShouldBe("FirstName2");
            dictionary["bill_to_surname"].ShouldBe("LastName2");
            dictionary["bill_to_address_country"].ShouldBe("US");
            dictionary["bill_to_address_state"].ShouldBe("CA");
            dictionary["signed_field_names"].ShouldBe("signed_field_names,transaction_type,reference_number,amount,currency,access_key,profile_id,transaction_uuid,signed_date_time,unsigned_field_names,locale,bill_to_email,bill_to_forename,bill_to_surname,bill_to_address_country,bill_to_address_state");
        }

        [Fact]
        public async Task TestLinkSetsViewbagSignature()
        {
            // Arrange
            Dictionary<string, string> dictionary = null;
            MockDataSigningService.Setup(a => a.Sign(It.IsAny<Dictionary<string, string>>())).Returns("FakeSign").Callback<Dictionary<string, string>>(c => dictionary = c);
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            Assert.IsType<ViewResult>(controllerResult);
            ((string)Controller.ViewBag.Signature).ShouldBe("FakeSign");
        }

        [Fact]
        public async Task TestLinkSetsPaymentDictionary()
        {
            // Arrange
            Dictionary<string, string> dictionary = null;
            MockDataSigningService.Setup(a => a.Sign(It.IsAny<Dictionary<string, string>>())).Returns("FakeSign").Callback<Dictionary<string, string>>(c => dictionary = c);
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;
            var od = OrderData[1].GetOrderDetails();
            od.AdjustmentAmount = 2.51m;
            OrderData[1].SaveDetails(od);

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderResultsModel>(result.Model);
            modelResult.PaymentDictionary.Count.ShouldBe(16);
            foreach (var dictItem in dictionary)
            {
                modelResult.PaymentDictionary[dictItem.Key].ShouldBe(dictItem.Value);
            }
        }

        [Fact]
        public async Task TestLinkSetsCyberSourceUrl()
        {
            // Arrange
            Dictionary<string, string> dictionary = null;
            MockDataSigningService.Setup(a => a.Sign(It.IsAny<Dictionary<string, string>>())).Returns("FakeSign").Callback<Dictionary<string, string>>(c => dictionary = c);
            OrderData[1].Status = OrderStatusCodes.Finalized;
            OrderData[1].PaymentType = PaymentTypeCodes.CreditCard;
            OrderData[1].Paid = false;

            // Act
            var controllerResult = await Controller.Link(OrderData[1].ShareIdentifier);

            // Assert
            var result = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<OrderResultsModel>(result.Model);
            modelResult.CyberSourceUrl.ShouldBe("Http://FakeUrl.com");
        }

        [Fact]
        public async Task TestDownloadReturnsNotFound()
        {
            // Arrange
            

            // Act
            var controllerResult = await Controller.Download(SpecificGuid.GetGuid(15));

            // Assert
            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task TestDownloadCallsFileStorageToGetRedirectUrl()
        {
            // Arrange
            var response = new SasResponse();
            response.AccessUrl = "Http://FakeUrl";
            MockFileStorageService.Setup(a => a.GetSharedAccessSignature(It.IsAny<string>())).ReturnsAsync(response);

            OrderData[1].ResultsFileIdentifier = "FakeId";

            // Act
            var controllerResult = await Controller.Download(OrderData[1].ShareIdentifier);

            // Assert
            MockFileStorageService.Verify(a => a.GetSharedAccessSignature("FakeId"), Times.Once);
            var redirctResult = Assert.IsType<RedirectResult>(controllerResult);
            redirctResult.Url.ShouldBe("Http://FakeUrl");
        }


    }
    [Trait("Category", "Controller Reflection")]
    public class ResultsControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public ResultsControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(ResultsController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(2);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(2);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(4);
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
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Link", 1 + countAdjustment, "Link-1", showListOfAttributes: false);

            //2
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Download", 1 + countAdjustment, "Download-1", showListOfAttributes: false);

            //3
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ConfirmPayment", 2 + countAdjustment, "ConfirmPaymentGet-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("ConfirmPayment", 2 + countAdjustment, "ConfirmPaymentGet-1", showListOfAttributes: false);

            //4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ConfirmPayment", 2 + countAdjustment, "ConfirmPaymentPost-1", true, showListOfAttributes: true);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("ConfirmPayment", 2 + countAdjustment, "ConfirmPaymentPost-1", true, showListOfAttributes: false);

        }

    }
}
