using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Anlab.Core.Models;

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

            if (populateAllFields)
            {                
                rtValue.LabId = string.Format("LabId{0}", counter);
                rtValue.ClientId = string.Format("ClientId{0}", counter);
                rtValue.AdditionalEmails = string.Format("AdditionalEmails{0}", counter);
                rtValue.JsonDetails = string.Format("JsonDetails{0}", counter);
                rtValue.Created = DateTime.Now;
                rtValue.Updated = DateTime.Now;
            }


            rtValue.Id = counter ?? 99;

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
            //Add more if needed

            return rtValue;
        }
    }
}
