using Anlab.Jobs.MoneyMovement;
using System.Collections.Generic;
using Anlab.Core.Models;
using TestHelpers.Helpers;
using Xunit;

namespace Test.TestsModel
{
    [Trait("Category", "ModelTests")]
    public class CoreModelTests
    {
        [Fact]
        public void TestSlothResponseModelFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("KfsTrackingNumber", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Message", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Status", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Success", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(SlothResponseModel));
        }
        [Fact]
        public void TestAccountModelFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Account", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Chart", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubAccount", "System.String", new List<string>()));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(AccountModel));
        }
        [Fact]
        public void TestClientDetailsLookupModelFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("ClientId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CopyEmail", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CopyPhone", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("DefaultAccount", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Department", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubEmail", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubPhone", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ClientDetailsLookupModel));
        }
    }
}
