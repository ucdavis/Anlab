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
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class ImpersonationControllerTests
    {
        //TODO
    }

    [Trait("Category", "Controller Reflection")]
    public class ImpersonationControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ImpersonationControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        protected readonly Type ControllerClass = typeof(ImpersonationController);

        #region Controller Class Tests
        [Fact]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            controllerClass.BaseType.ShouldNotBe(null);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            result.ShouldBe("ApplicationController");

            #endregion Assert
        }

        [Fact]
        public void TestControllerExpectedNumberOfAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in result)
            {
                output.WriteLine(o.ToString()); //Output shows 
            }
            result.Count().ShouldBe(3);

            #endregion Assert
        }

        /// <summary>
        /// #1
        /// </summary>
        [Fact]
        public void TestControllerHasControllerAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ControllerAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "ControllerAttribute not found.");

            #endregion Assert
        }

        /// <summary>
        /// #2
        /// </summary>
        [Fact]
        public void TestControllerHasAutoValidateAntiforgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "AutoValidateAntiforgeryTokenAttribute not found.");

            #endregion Assert
        }
        /// <summary>
        /// #3
        /// </summary>
        [Fact]
        public void TestControllerHasAuthorizeAttributeAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass.GetTypeInfo();
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            result.Count().ShouldBeGreaterThan(0, "AuthorizeAttribute not found.");
            result.ElementAt(0).Roles.ShouldBeNull();
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        [Fact]//(Skip = "Tests are still being written. When done, remove this line.")]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            result.Count().ShouldBe(2);

            #endregion Assert
        }

        [Fact]
        public void TestControllerMethodImpersonateUserContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("ImpersonateUser");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in allAttributes)
            {
                output.WriteLine(o.ToString()); //Output shows if the test fails
            }
#if DEBUG
            allAttributes.Count().ShouldBe(3, "No Attributes");
#else
            allAttributes.Count().ShouldBe(2, "No Attributes");
#endif
            expectedAttribute.Count().ShouldBe(1, "AuthorizeAttribute not found");
            expectedAttribute.ElementAt(0).Roles.ShouldBe(RoleCodes.Admin);
            #endregion Assert
        }


        [Fact]
        public void TestControllerMethodImpersonateUserContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("ImpersonateUser");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AsyncStateMachineAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in allAttributes)
            {
                output.WriteLine(o.ToString()); //Output shows if the test fails
            }
#if DEBUG
            allAttributes.Count().ShouldBe(3, "No Attributes");
#else
            allAttributes.Count().ShouldBe(2, "No Attributes");
#endif
            expectedAttribute.Count().ShouldBe(1, "AsyncStateMachineAttribute not found");
            #endregion Assert
        }
#if DEBUG
        [Fact]
        public void TestControllerMethodImpersonateUserContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("ImpersonateUser");
            #endregion Arrange

            #region Act
            
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<DebuggerStepThroughAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            foreach (var o in allAttributes)
            {
                output.WriteLine(o.ToString()); //Output shows if the test fails
            }

            allAttributes.Count().ShouldBe(3, "No Attributes");
            expectedAttribute.Count().ShouldBe(1, "DebuggerStepThroughAttribute not found");
            #endregion Assert
        }
#endif
        #endregion Controller Method Tests
    }

}
