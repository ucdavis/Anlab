using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            OrderData[2].Status = value;

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

            }

            Controller.ViewBag.ShowComplete = showComplete;
        }



        //TODO
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
        }


    }
}
