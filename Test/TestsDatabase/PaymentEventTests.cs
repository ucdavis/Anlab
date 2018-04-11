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
    public class PaymentEventTests
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
            expectedFields.Add(new NameAndType("Auth_Amount", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Decision", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("OccuredAt", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Reason_Code", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Req_Reference_Number", "System.Int32", new List<string>())); //Order Id
            expectedFields.Add(new NameAndType("ReturnedResults", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Transaction_Id", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.KeyAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(PaymentEvent));

        }

        #endregion Reflection of Database.	
    }
}
