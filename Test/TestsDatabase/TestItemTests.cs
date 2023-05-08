using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Test.Helpers;
using TestHelpers.Helpers;
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
            expectedFields.Add(new NameAndType("AdditionalInfoPrompt", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Analysis", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)512)]"
            }));
            expectedFields.Add(new NameAndType("Categories", "System.String[]", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Category", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)64)]"
            }));
            expectedFields.Add(new NameAndType("Group", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)512)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Code\")]",
                "[System.ComponentModel.DataAnnotations.KeyAttribute()]",
                "[System.ComponentModel.DataAnnotations.RegularExpressionAttribute(\"([A-Z0-9a-z\\-#_%])+\", ErrorMessage = \"Codes can only contain alphanumerics, #, _, %, and dashes.\")]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)128)]"                
            }));
            expectedFields.Add(new NameAndType("LabOrder", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("NotesEncoded", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Public", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Reporting", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("RequestOrder", "System.Int32", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TestItem));

        }

        #endregion Reflection of Database.	
    }
}
