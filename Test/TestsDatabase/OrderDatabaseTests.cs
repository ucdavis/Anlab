using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Test.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category", "DatabaseTests")]
    public class OrderDatabaseTests
    {
        [Fact]
        public void OrdersCanBeWrittenToDatabaseWithExistingUser()
        {
            using (var contextHelper = new ContextHelper())
            {

                contextHelper.Context.Orders.Count().ShouldBe(0);

                contextHelper.Context.Users.Add(CreateValidEntities.User(5));
                contextHelper.Context.SaveChanges();

                var order = CreateValidEntities.Order(1);
                order.Creator = contextHelper.Context.Users.FirstOrDefault();
                contextHelper.Context.Orders.Add(order);
                contextHelper.Context.SaveChanges();

                var updatedOrders = contextHelper.Context.Orders.Include(a => a.Creator).ToList();
                contextHelper.Context.Users.Count().ShouldBe(1);
                updatedOrders.Count().ShouldBe(1);


                updatedOrders[0].Creator.FirstName.ShouldBe("FirstName5");
            }
        }

        [Fact]
        public void OrdersCanBeWrittenToDatabaseWithNewUser()
        {
            using (var contextHelper = new ContextHelper())
            {
                contextHelper.Context.Orders.Count().ShouldBe(0);

                contextHelper.Context.Users.Count().ShouldBe(0);


                contextHelper.Context.Users.Add(CreateValidEntities.User(5));
                contextHelper.Context.SaveChanges();

                var order = CreateValidEntities.Order(1);
                order.Creator = CreateValidEntities.User(3);
                contextHelper.Context.Orders.Add(order);
                contextHelper.Context.SaveChanges();

                var updatedOrders = contextHelper.Context.Orders.Include(a => a.Creator).ToList();
                contextHelper.Context.Users.Count().ShouldBe(2);
                updatedOrders.Count().ShouldBe(1);

                updatedOrders[0].Creator.FirstName.ShouldBe("FirstName3");
            }
        }


        [Fact]
        public async Task TestTestAsyncSave()
        {
            using (var contextHelper = new ContextHelper())
            {

                (await contextHelper.Context.Orders.CountAsync()).ShouldBe(0);

                await contextHelper.Context.Users.AddAsync(CreateValidEntities.User(1));
                await contextHelper.Context.SaveChangesAsync();

                var order = CreateValidEntities.Order(1);
                order.Creator = await contextHelper.Context.Users.FirstOrDefaultAsync();
                await contextHelper.Context.Orders.AddAsync(order);
                await contextHelper.Context.SaveChangesAsync();

                (await contextHelper.Context.Orders.CountAsync()).ShouldBe(1);                
            }

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void OrdersCanBeWrittenToDatabaseWithTheoryWithHelper(int value)
        {
            using (var contextHelper = new ContextHelper())
            {
                contextHelper.Context.Orders.Count().ShouldBe(0);

                contextHelper.Context.Users.Add(CreateValidEntities.User(1));
                contextHelper.Context.SaveChanges();

                for (int i = 0; i < value; i++)
                {

                    var order = CreateValidEntities.Order(i + 1);
                    order.Creator = contextHelper.Context.Users.FirstOrDefault();
                    contextHelper.Context.Orders.Add(order);
                }


                contextHelper.Context.SaveChanges();

                contextHelper.Context.Orders.Count().ShouldBe(value);

            }

        }

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
            expectedFields.Add(new NameAndType("AdditionalEmails", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ApprovedPayment", "Anlab.Core.Domain.PaymentEvent", new List<string>()));
            expectedFields.Add(new NameAndType("ClientId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("ClientName", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)512)]"
            }));
            expectedFields.Add(new NameAndType("Created", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Creator", "Anlab.Core.Domain.User", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute(\"CreatorId\")]"
            }));
            expectedFields.Add(new NameAndType("CreatorId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)450)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("IsDeleted", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("JsonDetails", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("KfsTrackingNumber", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)20)]"
            }));
            expectedFields.Add(new NameAndType("LabId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("MailMessages", "System.Collections.Generic.ICollection`1[Anlab.Core.Domain.MailMessage]", new List<string>()));
            expectedFields.Add(new NameAndType("Paid", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("PaymentType", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Project", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"
            }));
            expectedFields.Add(new NameAndType("RequestNum", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("ResultsFileIdentifier", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SavedTestDetails", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ShareIdentifier", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("SlothTransactionId", "System.Nullable`1[System.Guid]", new List<string>()));
            expectedFields.Add(new NameAndType("Status", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Updated", "System.DateTime", new List<string>()));
            
            
            


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Order));

        }

        #endregion Reflection of Database.	
    }
}
