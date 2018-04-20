using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Test.Helpers;
using Test.TestsDatabase;
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

        //Setup Data
        public List<Order> OrderData { get; set; }
        public List<TestItemModel> TestItemModelData { get; set; }


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
                OrderData.Add(CreateValidEntities.Order(i + 1));
            }

            //Setups
            MockHttpContext.Setup(m => m.User).Returns(user);
            MockDbContext.Setup(m => m.Orders).Returns(OrderData.AsQueryable().MockAsyncDbSet().Object);
            MockOrderService.Setup(a => a.PopulateTestItemModel(It.IsAny<bool>())).ReturnsAsync(TestItemModelData);

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
                }
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
            var model = Assert.IsType<Order[]>(result.Model);
            model.Length.ShouldBe(2);
            model[0].CreatorId.ShouldBe("Creator1");
            model[1].CreatorId.ShouldBe("Creator1");        
        }

       


        [Fact]
        public async Task CreateCallsOrderService()
        {

            

            var proc = new TestItemPrices();
            proc.Id = "PROC";
            proc.InternalCost = 6m;
            MockLabworksService.Setup(a => a.GetPrice("PROC")).ReturnsAsync(proc);
            var appSettings = new AppSettings();
            appSettings.NonUcRate = 1.9m;

            MockAppSettings.Setup(a => a.Value).Returns(appSettings);

            var users = new List<User>()
            {
                CreateValidEntities.User(1, true)
            };
            users[0].Id = "Creator1";

            MockDbContext.Setup(a => a.Users).Returns(users.AsQueryable().MockAsyncDbSet().Object);

            var controller = new OrderController(MockDbContext.Object,
                MockOrderService.Object,
                MockOrderMessagingService.Object,
                MockLabworksService.Object,
                MockFinancialService.Object,
                MockAppSettings.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = MockHttpContext.Object
            };

            var controllerResult = await controller.Create();
        }


        //TODO
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
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmation", 2 + countAdjustment, "CopyPost-1",true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Confirmation", 2 + countAdjustment, "CopyPost-2",true , showListOfAttributes: false);

            //9
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Confirmed", 1 + countAdjustment, "Confirmed-1", showListOfAttributes: false);

            //10
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Delete", 2 + countAdjustment, "Delete-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Delete", 2 + countAdjustment, "Delete-2", showListOfAttributes: false);

            //11
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("LookupClientId", 2 + countAdjustment, "LookupClientId-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("LookupClientId", 2 + countAdjustment, "LookupClientId-2", showListOfAttributes: false);

        }

    }
}
