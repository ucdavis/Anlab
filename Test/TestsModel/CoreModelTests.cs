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

        [Fact]
        public void TestClientInfoFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("ClientId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CopyPhone", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Department", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Employer", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PhoneNumber", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ClientInfo));
        }

        [Fact]
        public void TestIDatedEntityFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Created", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Updated", "System.DateTime", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(IDatedEntity));
        }
        [Fact]
        public void TestEmailSettingsFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AnlabAddress", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Host", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Password", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Port", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("UserName", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailSettings));
        }

        [Fact]
        public void TestFinancialSettingsFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AnlabAccount", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CreditObjectCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("DebitObjectCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SlothApiKey", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SlothApiUrl", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(FinancialSettings));
        }

        [Fact]
        public void TestLabFinalizeModelFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AdjustmentAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("BypassEmail", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Confirm", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LabComments", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("UploadFile", "Microsoft.AspNetCore.Http.IFormFile", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(LabFinalizeModel));
        }
    }
}
