using Anlab.Core.Domain;
using System.Collections.Generic;
using Test.Helpers;
using Xunit;

namespace Test.TestsModel
{
    [Trait("Category", "ModelTests")]
    public class OrderModelTests
    {
        [Fact (Skip = "This is just an example and is covered in the database test")]
        public void TestFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AdditionalEmails", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ApprovedPayment", "Anlab.Core.Domain.PaymentEvent", new List<string>()));
            expectedFields.Add(new NameAndType("ClientId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("Created", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Creator", "Anlab.Core.Domain.User", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute(\"CreatorId\")]"
            }));
            expectedFields.Add(new NameAndType("CreatorId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("JsonDetails", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LabId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("MailMessages", "System.Collections.Generic.ICollection`1[Anlab.Core.Domain.MailMessage]", new List<string>()));
            expectedFields.Add(new NameAndType("Paid", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Project", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"
                
            }));
            expectedFields.Add(new NameAndType("RequestNum", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ResultsFileIdentifier", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ShareIdentifier", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("Status", "System.String", new List<string>()));            
            expectedFields.Add(new NameAndType("Updated", "System.DateTime", new List<string>())); 
            
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Order));
        }

    }
}
