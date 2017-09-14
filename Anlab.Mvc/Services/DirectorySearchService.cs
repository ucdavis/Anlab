using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.Configuration;
using AzureActiveDirectorySearcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services
{
    public interface IDirectorySearchService
    {
        Task<GraphUser> GetByKerb(string kerb);
        Task<GraphUser> GetByEmail(string email);
    }

    public class DirectorySearchService : IDirectorySearchService
    {
        private readonly GraphSearchClient _client;

        public DirectorySearchService(IOptions<AzureOptions> configuration)
        {
            var azureOptions = configuration.Value;
            _client = new GraphSearchClient(new ActiveDirectoryConfigurationValues(azureOptions.TenantName,
                azureOptions.TentantId, azureOptions.ClientId, azureOptions.ClientSecret));
        }
        public Task<GraphUser> GetByKerb(string kerb)
        {
            return _client.GetUserByKerberos(kerb);
        }

        public Task<GraphUser> GetByEmail(string email)
        {
            return _client.GetUserByEmail(email);
        }
    }
}
