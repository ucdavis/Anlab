using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Test.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category", "DatabaseTests")]
    public class AnalysisMethodTests
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
            expectedFields.Add(new NameAndType("Category", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"
            }));
            expectedFields.Add(new NameAndType("Content", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute((System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption)0)]"
            }));
            expectedFields.Add(new NameAndType("Title", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"
            }));


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(AnalysisMethod));

        }

        #endregion Reflection of Database.	
    }
}
