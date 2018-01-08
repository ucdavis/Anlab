using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models;
using AzureActiveDirectorySearcher;
using Ietws;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services
{
    public interface IDirectorySearchService
    {
        Task<Person> GetByEmailAsync(string email);
    }

    public class IetWsSearchService : IDirectorySearchService
    {
        private readonly IetClient ietClient;

        public IetWsSearchService(IOptions<AppSettings> configuration)
        {
            var settings = configuration.Value;
            // TODO: get key from settings
            ietClient = new IetClient("");
        }

        public async Task<Person> GetByEmailAsync(string email)
        {
            var ucdContactResult = await ietClient.Contacts.Search(ContactSearchField.email, email);
            var ucdContact = ucdContactResult.ResponseData.Results.First();

            var ucdKerbResult = await ietClient.Kerberos.Search(KerberosSearchField.iamId, ucdContact.IamId);
            var ucdKerbPerson = ucdKerbResult.ResponseData.Results.Single();

            return new Person
            {
                GivenName = ucdKerbPerson.OFirstName,
                Surname = ucdKerbPerson.OLastName,
                FullName = ucdKerbPerson.OFullName,
                Kerberos = ucdKerbPerson.UserId,
                Mail = ucdContact.Email
            };
        }
    }
}
