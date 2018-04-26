using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AnlabMvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    public class PaymentControllerTests
    {
        //TODO
    }
    [Trait("Category", "Controller Reflection")]
    public class PaymentControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public PaymentControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(PaymentController));
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
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Receipt", 2, "Receipt-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<IgnoreAntiforgeryTokenAttribute>("Receipt", 2, "Receipt-2", showListOfAttributes: false);

            //2
            ControllerReflection.MethodExpectedAttribute<IgnoreAntiforgeryTokenAttribute>("PaymentError", 1, "PaymentError", showListOfAttributes: false);

            //3
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Cancel", 2, "Cancel-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<IgnoreAntiforgeryTokenAttribute>("Cancel", 2, "Cancel-2", showListOfAttributes: false);

            //4
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("ProviderNotify", 4 + countAdjustment, "ProviderNotify-1", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ProviderNotify", 4 + countAdjustment, "ProviderNotify-2", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AllowAnonymousAttribute>("ProviderNotify", 4 + countAdjustment, "ProviderNotify-3", showListOfAttributes: false);            
            ControllerReflection.MethodExpectedAttribute<IgnoreAntiforgeryTokenAttribute>("ProviderNotify", 4 + countAdjustment, "ProviderNotify-4", showListOfAttributes: false);

        }

    }
}
