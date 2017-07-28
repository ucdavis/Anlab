using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Test.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category", "DatabaseTests")]
    public class TestItemTests
    {
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [Fact]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Analysis", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)512)]"
            }));
            expectedFields.Add(new NameAndType("Category", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)64)]"
            }));
            expectedFields.Add(new NameAndType("ChargeSet", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ExternalCost", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("FeeSchedule", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)7)]"
            }));
            expectedFields.Add(new NameAndType("Group", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)8)]"
            }));
            expectedFields.Add(new NameAndType("GroupType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.KeyAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)128)]"
            }));
            expectedFields.Add(new NameAndType("InternalCost", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Multiplier", "System.Int32", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RangeAttribute((Int32)0, (Int32)2147483647)]"
            }));
            expectedFields.Add(new NameAndType("Multiplies", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Public", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("SetupCost", "System.Decimal", new List<string>()));


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TestItem));

        }

        #endregion Reflection of Database.	
    }
}
