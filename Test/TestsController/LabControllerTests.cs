using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AnlabMvc.Controllers;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class LabControllerTests
    {
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
            ControllerReflection = new ControllerReflection(this.output);
        }
        protected readonly Type ControllerClass = typeof(LabController);
        

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits(ControllerClass, "ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(ControllerClass, 3);
            authAttribute.ElementAt(0).Roles.ShouldBe($"{RoleCodes.Admin},{RoleCodes.LabUser}");

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(ControllerClass, 3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(ControllerClass, 3);
        }


        [Fact]
        public void TestControllerMethodAttributes()
        {
#if DEBUG
            var countAdjustment = 1;
#else
            var countAdjustment = 0;
#endif
            output.WriteLine("Test 1");
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>(ControllerClass, "Orders", 1);
            output.WriteLine("Test 2");
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>(ControllerClass, "Details", 1 + countAdjustment);
            output.WriteLine("Test 3");
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>(ControllerClass, "AddRequestNumber", 1 + countAdjustment, false);
            output.WriteLine("Test 4");
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>(ControllerClass, "AddRequestNumber", 2 + countAdjustment, true);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>(ControllerClass, "AddRequestNumber", 2 + countAdjustment, true);
            output.WriteLine("Test 5");
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>(ControllerClass, "Confirmation", 1 + countAdjustment, false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>(ControllerClass, "Confirmation", 2 + countAdjustment, true);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>(ControllerClass, "Confirmation", 2 + countAdjustment, true);
        }


    }
}
