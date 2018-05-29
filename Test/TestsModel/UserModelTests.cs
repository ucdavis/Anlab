using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Test.Helpers;
using TestHelpers.Helpers;
using Xunit;

namespace Test.TestsModel
{
    [Trait("Category", "ModelTests")]
    public class UserModelTests
    {
        [Fact(Skip = "Ignore for now")]
        public void TestFieldsHaveExpectedAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("AccessFailedCount", "System.Int32", new List<string>()));

            expectedFields.Add(new NameAndType("Account", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("Claims", "System.Collections.Generic.ICollection`1[Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim`1[System.String]]", new List<string>()));
            expectedFields.Add(new NameAndType("ClientId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("ConcurrencyStamp", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("EmailConfirmed", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("FirstName", "System.String", new List<string>
            {                
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"First Name\")]",                
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"

            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LastName", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Last Name\")]",                
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"

            }));
            expectedFields.Add(new NameAndType("LockoutEnabled", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LockoutEnd", "System.Nullable`1[System.DateTimeOffset]", new List<string>()));
            expectedFields.Add(new NameAndType("Logins", "System.Collections.Generic.ICollection`1[Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin`1[System.String]]", new List<string>()));

            expectedFields.Add(new NameAndType("MailMessages", "System.Collections.Generic.ICollection`1[Anlab.Core.Domain.MailMessage]", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Name\")]",
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"

            }));
            expectedFields.Add(new NameAndType("NormalizedEmail", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("NormalizedUserName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PasswordHash", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Phone", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PhoneNumber", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PhoneNumberConfirmed", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Roles", "System.Collections.Generic.ICollection`1[Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole`1[System.String]]", new List<string>()));
            expectedFields.Add(new NameAndType("SecurityStamp", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("TwoFactorEnabled", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("UserName", "System.String", new List<string>()));
            
            
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(User));
        }
    }
}
