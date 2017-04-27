using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;

namespace Test.Helpers
{
    public static class CreateValidEntities
    {
        public static Order Order(int? counter, bool populateAllFields = false)
        {
            var rtValue = new Order();
            rtValue.CreatorId = string.Format("CreatorId{0}", counter);
            rtValue.Project = string.Format("Project{0}", counter);

            if (populateAllFields)
            {
                rtValue.Creator = new User(); //Meh? test later
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

            return rtValue;

        }
    }
}
