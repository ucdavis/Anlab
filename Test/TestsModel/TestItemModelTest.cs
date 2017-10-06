using System.Collections.Generic;
using Anlab.Core.Domain;
using Test.Helpers;
using Xunit;

namespace Test.TestsModel
{
    [Trait("Category", "ModelTests")]
    public class TestItemModelTests
    {
        [Fact]
        public void TestFieldsHaveExpectedAttributes()
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
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Code\")]",
                "[System.ComponentModel.DataAnnotations.KeyAttribute()]",
                "[System.ComponentModel.DataAnnotations.RegularExpressionAttribute(\"([A-Z0-9a-z\\-#])+\", ErrorMessage = \"Codes can only contain alphanumerics, #, and dashes.\")]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)128)]"                
            }));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("NotesEncoded", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Public", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TestItem));
        }

    }
}
