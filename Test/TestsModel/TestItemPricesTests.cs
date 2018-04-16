using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Models;
using Shouldly;
using TestHelpers.Helpers;
using Xunit;

namespace Test.TestsModel
{
    public class TestItemPricesTests
    {
        #region Cost Tests

        [Fact]
        public void CostReturnsExpectedValues1()
        {
            var theThing = new TestItemPrices();
            theThing.Id = "prOc";
            theThing.InternalCost = 2;
            theThing.Noninv = true;
            theThing.Nonrep = true;

            theThing.Cost.ShouldBe(2);
        }

        [Fact]
        public void CostReturnsExpectedValues2()
        {
            var theThing = new TestItemPrices();
            theThing.Id = "xxx";
            theThing.InternalCost = 2;
            theThing.Noninv = true;
            theThing.Nonrep = true;

            theThing.Cost.ShouldBe(0);
        }

        [Fact]
        public void CostReturnsExpectedValues3()
        {
            var theThing = new TestItemPrices();
            theThing.Id = "xxx";
            theThing.InternalCost = 2;
            theThing.Noninv = false;
            theThing.Nonrep = true;

            theThing.Cost.ShouldBe(2);
        }
        [Fact]
        public void CostReturnsExpectedValues4()
        {
            var theThing = new TestItemPrices();
            theThing.Id = "xxx";
            theThing.InternalCost = 2;
            theThing.Noninv = true;
            theThing.Nonrep = false;

            theThing.Cost.ShouldBe(2);
        }
        [Fact]
        public void CostReturnsExpectedValues5()
        {
            var theThing = new TestItemPrices();
            theThing.Id = "xxx";
            theThing.InternalCost = 2;
            theThing.Noninv = false;
            theThing.Nonrep = false;

            theThing.Cost.ShouldBe(2);
        }

        #endregion Cost Tests

        #region SetupPrice Tests
        [Fact]
        public void SetupPriceReturnsExpectedValues1()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 2;
            theThing.Multiplier = 3;
            theThing.Nonrep = true;

            theThing.SetupPrice.ShouldBe(0);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues2()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 2;
            theThing.Multiplier = 3;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(6);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues3()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 2;
            theThing.Multiplier = 4;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(8);
        }

        [Fact]
        public void SetupPriceReturnsExpectedValues4()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 1.49m;
            theThing.Multiplier = 1;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(2);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues5()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 1.44m;
            theThing.Multiplier = 1;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(2);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues6()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 1.0001m;
            theThing.Multiplier = 1;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(2);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues7()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 1.000m;
            theThing.Multiplier = 1;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(1);
        }
        [Fact]
        public void SetupPriceReturnsExpectedValues8()
        {
            var theThing = new TestItemPrices();
            theThing.SetupCost = 0.99m;
            theThing.Multiplier = 1;
            theThing.Nonrep = false;

            theThing.SetupPrice.ShouldBe(1);
        }
        #endregion SetupPrice Tests


        [Fact]
        public void TestTestItemPricesFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Cost", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("InternalCost", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Multiplier", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Noninv", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Nonrep", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("SetupCost", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("SetupPrice", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Sop", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TestItemPrices));
        }
    }
}
