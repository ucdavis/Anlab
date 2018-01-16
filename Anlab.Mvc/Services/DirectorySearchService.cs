﻿using System;
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
        Task<Person> GetByEmail(string email);
    }

    public class IetWsSearchService : IDirectorySearchService
    {
        private readonly IetClient ietClient;

        public IetWsSearchService(IOptions<AppSettings> configuration)
        {
            var settings = configuration.Value;
            ietClient = new IetClient(settings.IetWsKey);
        }

        public async Task<Person> GetByEmail(string email)
        {
            // find the contact via their email
            var ucdContactResult = await ietClient.Contacts.Search(ContactSearchField.email, email);
            EnsureResponseSuccess(ucdContactResult);
            var ucdContact = ucdContactResult.ResponseData.Results.First();

            // now look up the whole person's record by ID including kerb
            var ucdKerbResult = await ietClient.Kerberos.Search(KerberosSearchField.iamId, ucdContact.IamId);
            EnsureResponseSuccess(ucdKerbResult);
            var ucdKerbPerson = ucdKerbResult.ResponseData.Results.Single();
            return new Person
            {
                GivenName = ucdKerbPerson.DFirstName,
                Surname = ucdKerbPerson.DLastName,
                FullName = ucdKerbPerson.DFullName,
                Kerberos = ucdKerbPerson.UserId,
                Mail = ucdContact.Email
            };
        }

        private void EnsureResponseSuccess<T>(IetResult<T> result) {
            if (result.ResponseStatus != 0)
            {
                throw new ApplicationException(result.ResponseDetails);
            }
        }
    }
}
