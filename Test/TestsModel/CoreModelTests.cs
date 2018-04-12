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

        [Fact]
        public void TestLabReceiveModelFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AdjustmentAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("BypassEmail", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Confirm", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LabComments", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(LabReceiveModel));
        }

        [Fact]
        public void TestOrderDetailsFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AdditionalEmails", "System.String[]", new List<string>()));
            expectedFields.Add(new NameAndType("AdditionalInfo", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]"
            }));
            expectedFields.Add(new NameAndType("AdditionalInfoList", "System.Collections.Generic.Dictionary`2[System.String,System.String]", new List<string>()));
            expectedFields.Add(new NameAndType("AdjustmentAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("ClientInfo", "Anlab.Core.Models.ClientInfo", new List<string>()));
            expectedFields.Add(new NameAndType("Commodity", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("DateSampled", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("ExternalProcessingFee", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("GrandTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("InternalProcessingFee", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("LabComments", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]"
            }));
            expectedFields.Add(new NameAndType("LabworksSampleDisposition", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("OtherPaymentInfo", "Anlab.Core.Models.OtherPaymentInfo", new List<string>()));
            expectedFields.Add(new NameAndType("Payment", "Anlab.Core.Models.Payment", new List<string>()));
            expectedFields.Add(new NameAndType("Project", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Quantity", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("RushMultiplier", "System.Nullable`1[System.Decimal]", new List<string>()));
            expectedFields.Add(new NameAndType("SampleDisposition", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SampleType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SampleTypeQuestions", "Anlab.Core.Models.SampleTypeQuestions", new List<string>()));
            expectedFields.Add(new NameAndType("SelectedTests", "System.Collections.Generic.IList`1[Anlab.Core.Models.TestDetails]", new List<string>()));
            expectedFields.Add(new NameAndType("Total", "System.Decimal", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderDetails));
        }
    }
}
