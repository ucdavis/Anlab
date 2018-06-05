using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using TestHelpers.Helpers;

namespace Test.Helpers
{
    public static class CreateValidEntities
    {
        public static Order Order(int? counter, bool populateAllFields = false)
        {
            var rtValue = new Order();
            rtValue.CreatorId = string.Format("CreatorId{0}", counter);
            rtValue.Project = string.Format("Project{0}", counter);
            rtValue.Creator = new User(); //Meh? test later
            rtValue.ShareIdentifier = SpecificGuid.GetGuid(counter ?? 99);

            if (populateAllFields)
            {                
                rtValue.LabId = string.Format("LabId{0}", counter);
                rtValue.ClientId = string.Format("ClientId{0}", counter);
                rtValue.AdditionalEmails = string.Format("AdditionalEmails{0}", counter);
                var orderDetails = CreateValidEntities.OrderDetails(counter);
                rtValue.SaveDetails(orderDetails);
                rtValue.Created = DateTime.Now;
                rtValue.Updated = DateTime.Now;
            }


            rtValue.Id = counter ?? 99;

            return rtValue;
        }

        public static OrderDetails OrderDetails(int? counter, bool populateAllFields = false)
        {
            var rtValue = new OrderDetails();
            rtValue.Payment = new Payment();
            rtValue.OtherPaymentInfo = CreateValidEntities.OtherPaymentInfo(counter);
            rtValue.ClientInfo = new ClientInfo();
            rtValue.ClientInfo.ClientId = $"ClientId{counter}";

            return rtValue;
        }

        public static OtherPaymentInfo OtherPaymentInfo(int? counter)
        {
            var rtValue = new OtherPaymentInfo();
            rtValue.CompanyName = $"CompanyName{counter}";
            rtValue.AcAddr = $"AcAddr{counter}";
            rtValue.AcEmail = $"AcEmail{counter}@test.com";
            rtValue.AcName = $"AcName{counter}";
            rtValue.AcPhone = $"AcPhone{counter}";
            rtValue.PaymentType = $"PaymentType{counter}";
            rtValue.PoNum = $"PoNum{counter}";

            return rtValue;
        }

        public static User User(int? counter, bool populateAllFields = false)
        {
            var rtValue = new User();
            rtValue.FirstName = string.Format("FirstName{0}", counter);
            rtValue.LastName = string.Format("LastName{0}", counter);
            rtValue.Name = string.Format("{0} {1}", rtValue.FirstName, rtValue.LastName);
            rtValue.Email = $"test{counter}@testy.com";

            if (populateAllFields)
            {
                rtValue.ClientId = string.Format("ClientId{0}", counter);
                rtValue.Phone = string.Format("Phone{0}", counter);
                rtValue.Account = string.Format("Account{0}", counter);
                rtValue.Email = $"test{counter}@test.com";
                rtValue.CompanyName = $"CompanyName{counter}";
                rtValue.BillingContactAddress = $"BillingContactAddress{counter}";
                rtValue.BillingContactEmail = $"BillingContactEmail{counter}@test.com";
                rtValue.BillingContactName = $"BillingContactName{counter}";
                rtValue.BillingContactPhone = $"BillingContactPhone{counter}";
            }

            rtValue.Id = (counter ?? 99).ToString();

            return rtValue;

        }

        public static TestItemModel TestItemModel(int? counter, bool populateAllFields = false)
        {
            var rtValue = new TestItemModel();
            rtValue.Id = $"{counter ?? 99}";
            rtValue.Code = $"Code{counter ?? 99}";
            rtValue.Category = "Soil|Plant";
            rtValue.Public = true;
            //Add more if needed

            return rtValue;
        }

        public static ClientDetailsLookupModel ClientDetailsLookupModel(int? counter, bool populateAllFields = false)
        {
            var rtValue = new ClientDetailsLookupModel();
            rtValue.CopyEmail = $"CopyEmail{counter}@test.com";
            rtValue.ClientId = $"ClientId{counter}";
            rtValue.CopyPhone = $"CopyPhone{counter}";
            rtValue.DefaultAccount = $"DefaultAccount{counter}";
            rtValue.Department = $"Department{counter}";
            rtValue.Name = $"Name{counter}";
            rtValue.SubEmail = $"SubEmail{counter}@test.com";
            rtValue.SubPhone = $"SubPhone{counter}";

            return rtValue;

        }

        public static TestDetails TestDetails(int? counter)
        {
            var rtValue = new TestDetails();
            rtValue.Id = $"Id{counter}";
            rtValue.Analysis = $"Analysis{counter}";
            rtValue.Cost = 1.0m * counter ?? 9;
            rtValue.SetupCost = 2.0m * counter ?? 9;
            rtValue.SubTotal = 3.0m * counter ?? 9;
            rtValue.Total = 1.1m * counter ?? 9;

            return rtValue;
        }
    }
}
