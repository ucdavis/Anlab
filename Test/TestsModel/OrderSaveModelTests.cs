using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Test.TestsModel
{
    [Trait("Category", "ModelTests")]
    public class OrderSaveModelTests
    {
        [Fact]
        public void QuantityAllows200()
        {
            var results = Validate(CreateValidModel(200));

            results.ShouldBeEmpty();
        }

        [Fact]
        public void QuantityRejects201()
        {
            var results = Validate(CreateValidModel(201));

            results.Count.ShouldBe(1);
            results.Single().MemberNames.ShouldContain("Quantity");
            results.Single().ErrorMessage.ShouldBe("The field Quantity must be between 1 and 200.");
        }

        private static IList<ValidationResult> Validate(OrderSaveModel model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                results,
                validateAllProperties: true);

            return results;
        }

        private static OrderSaveModel CreateValidModel(int quantity)
        {
            return new OrderSaveModel
            {
                Quantity = quantity,
                SampleType = "Soil",
                SelectedTests = new[] { new TestDetails { Id = "PH-S" } },
                Project = "Project",
                DateSampled = DateTime.Today,
                SampleDisposition = "Dispose of my samples 30 days from report date."
            };
        }
    }
}
