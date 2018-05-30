using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
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


        //Setup Data
        public List<Order> OrderData { get; set; }

        //Controller
        public LabController Controller { get; set; }





        public LabControllerTests()
        {
            MockDbContext = new Mock<ApplicationDbContext>();
            MockHttpContext = new Mock<HttpContext>();
            MockOrderService = new Mock<IOrderService>();
            MockLabworksService = new Mock<ILabworksService>();
            MockOrderMessagingService = new Mock<IOrderMessageService>();
            MockFileStorageService = new Mock<IFileStorageService>();
            MockSlothService = new Mock<ISlothService>();

            var mockDataProvider = new Mock<SessionStateTempDataProvider>();



            //Default Data
            OrderData = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                var order = CreateValidEntities.Order(i + 1, true);
                order.Creator = CreateValidEntities.User(2);
                OrderData.Add(order);
            }



            //Setups
            MockDbContext.Setup(m => m.Orders).Returns(OrderData.AsQueryable().MockAsyncDbSet().Object);


            Controller = new LabController(MockDbContext.Object, MockOrderService.Object, MockLabworksService.Object,
                MockOrderMessagingService.Object, MockFileStorageService.Object, MockSlothService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = MockHttpContext.Object
                },
                TempData = new TempDataDictionary(MockHttpContext.Object, mockDataProvider.Object)
            };
        }


        [Theory]
        [InlineData(OrderStatusCodes.Confirmed, false)]
        [InlineData(OrderStatusCodes.Received, false)]
        [InlineData(OrderStatusCodes.Finalized, false)]
        [InlineData(OrderStatusCodes.Confirmed, true)]
        [InlineData(OrderStatusCodes.Received, true)]
        [InlineData(OrderStatusCodes.Finalized, true)]
        [InlineData(OrderStatusCodes.Complete, true)]
        [InlineData(OrderStatusCodes.Complete, false)]
        public void TestOrderReturnsExpectedResults1(string value, bool showComplete)
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
            var controllerResult = Controller.Orders(showComplete);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<Order>>(viewResult.Model);
            if (value == OrderStatusCodes.Complete && showComplete == false)
            {
                modelResult.Count.ShouldBe(0);
            }
            else
            {
                modelResult.Count.ShouldBe(2);
                modelResult[0].Id.ShouldBe(3); //Sorted by updated date
                modelResult[1].Id.ShouldBe(2); //Sorted by updated date

                modelResult[1].ClientId.ShouldBe("ClientId2");
                modelResult[1].Creator.ShouldNotBeNull();
                modelResult[1].Creator.Email.ShouldBe("test2@testy.com");
                modelResult[1].Created.ShouldNotBeNull();
                modelResult[1].Updated.ShouldNotBeNull();
                modelResult[1].RequestNum.ShouldBe("Blah");
                modelResult[1].Status.ShouldBe(value);
                modelResult[1].ShareIdentifier.ShouldBe(SpecificGuid.GetGuid(2));
                modelResult[1].Paid.ShouldBeFalse();

                modelResult[0].Paid.ShouldBeTrue();

                modelResult[1].ClientName.ShouldBe("Hup");
            }

            Controller.ViewBag.ShowComplete = showComplete;            

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestOrdersReturnsMax1000Orders(bool value)
        {
            // Arrange
            for (int i = 0; i < 1200; i++)
            {
                var order = CreateValidEntities.Order(i + 10);
                order.Status = OrderStatusCodes.Finalized;
                OrderData.Add(order);
            }


            // Act
            var controllerResult = Controller.Orders(value);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            var modelResult = Assert.IsType<List<Order>>(viewResult.Model);
            modelResult.Count.ShouldBe(1000);
        }

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
        public async Task TestAddRequestNumberPostUppersValueAndChecksForDups1()
        {
            // Arrange
            OrderData[0].RequestNum = "ABC123";
            OrderData[1].Status = OrderStatusCodes.Confirmed;

            // Act
            var cr = await Controller.AddRequestNumber(9, true, "AbC123");

            // Assert
#if DEBUG
            Assert.IsType<NotFoundResult>(cr); //Because it falls through to the check if the order exists.
#else
            var rr = Assert.IsType<RedirectToActionResult>(cr);
            rr.ActionName.ShouldBe("AddRequestNumber");
            rr.ControllerName.ShouldBeNull();
#endif
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

        [Fact(Skip = "Reminder to test the rest")]
        public void TestTheRestReminder()
        {
            // Arrange
            


            // Act


            // Assert		
        }
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
            ControllerReflection.ControllerPublicMethods(13);
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
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("Orders", 1, "Orders-1", showListOfAttributes:false);

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
        }


    }
}
