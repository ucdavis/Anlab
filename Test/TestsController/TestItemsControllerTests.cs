using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TestItemsControllerTests
    {
        //TODO
    }

    [Trait("Category", "Controller Reflection")]
    public class TestItemsControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public TestItemsControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(TestItemsController));
        }

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("ApplicationController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(3);
            authAttribute.ElementAt(0).Roles.ShouldNotBeNull();
            authAttribute.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenAttribute>(3);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(3);
        }

        [Fact]
        public void TestControllerMethodCount()
        {
            ControllerReflection.ControllerPublicMethods(8);
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
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Details", 1 + countAdjustment, "Details-1", showListOfAttributes: false);

            //3
            ControllerReflection.MethodExpectedNoAttribute("Create", "Create-1");
            //4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Create", 3 + countAdjustment, "Create-2",isSecondMethod: true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Create", 3 + countAdjustment, "Create-2", isSecondMethod: true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<ValidateAntiForgeryTokenAttribute>("Create", 3 + countAdjustment, "Create-2", isSecondMethod: true, showListOfAttributes: false); //It doesn't really need this because the class inherits the auto version

            //5
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Edit", 1 + countAdjustment, "Edit-1", showListOfAttributes: false);
            //6
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Edit", 3 + countAdjustment, "Edit-2", isSecondMethod: true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Edit", 3 + countAdjustment, "Edit-2", isSecondMethod: true, showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<ValidateAntiForgeryTokenAttribute>("Edit", 3 + countAdjustment, "Edit-2", isSecondMethod: true, showListOfAttributes: false); //It doesn't really need this because the class inherits the auto version

            //7
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Delete", 1 + countAdjustment, "Delete-1", showListOfAttributes: false);
            //8
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("DeleteConfirmed", 4 + countAdjustment, "DeleteConfirmed-2", showListOfAttributes: true);
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("DeleteConfirmed", 4 + countAdjustment, "DeleteConfirmed-2", showListOfAttributes: false);
            var result = ControllerReflection.MethodExpectedAttribute<ActionNameAttribute>("DeleteConfirmed", 4 + countAdjustment, "DeleteConfirmed-2", showListOfAttributes: false);
            result.Count().ShouldBe(1);
            result.ElementAt(0).Name.ShouldBe("Delete");            
            ControllerReflection.MethodExpectedAttribute<ValidateAntiForgeryTokenAttribute>("DeleteConfirmed", 4 + countAdjustment, "DeleteConfirmed-2", showListOfAttributes: false); //It doesn't really need this because the class inherits the auto version

        }

    }
}
