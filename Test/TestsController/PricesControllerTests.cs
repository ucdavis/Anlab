using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Models;
using AnlabMvc.Controllers;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class PricesControllerTests
    {
        //Mocks
        public Mock<IOrderService> MockOrderService { get; set; }

        //Setup Data
        private List<TestItemModel> TestItems { get; set; }

        //Controller
        public PricesController Controller { get; set; }

        public PricesControllerTests()
        {
            MockOrderService = new Mock<IOrderService>();

            TestItems = new List<TestItemModel>();
            for (int i = 0; i < 5; i++)
            {
                TestItems.Add(CreateValidEntities.TestItemModel(i+1));
            }
            TestItems[1].Public = false;

            MockOrderService.Setup(a => a.PopulateTestItemModel(It.IsAny<bool>())).ReturnsAsync(TestItems);

            //The controller
            Controller = new PricesController(MockOrderService.Object); //Don't need identity of view bag stuff
        }

        [Fact]
        public async Task TestIndexCallsPopulateTestItemModel()
        {
            // Arrange
            
            // Act
            await Controller.Index();

            // Assert
            MockOrderService.Verify(a => a.PopulateTestItemModel(false), Times.Once);
        }
        [Fact]
        public async Task TestIndexReturnsViewWithExpectedData()
        {
            // Arrange

            // Act
            var controllerResult = await Controller.Index();

            // Assert
            MockOrderService.Verify(a => a.PopulateTestItemModel(false), Times.Once);
            var result = Assert.IsType<ViewResult>(controllerResult);
            var resultModel = Assert.IsType<TestItemModel[]>(result.Model);
            resultModel.Length.ShouldBe(4);
            foreach (var testItemModel in resultModel)
            {
                testItemModel.Public.ShouldBe(true);
            }
        }
    }
    [Trait("Category", "Controller Reflection")]
    public class PricesControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public PricesControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(PricesController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("Controller");

            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(1);
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
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Index", 1 + countAdjustment, "Receipt-2", showListOfAttributes: true);

           
        }

    }
}
