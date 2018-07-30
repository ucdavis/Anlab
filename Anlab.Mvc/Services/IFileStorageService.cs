using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnlabMvc.Models.FileUploadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace AnlabMvc.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Returns a unique URL, including Shared Signature, which can be used to upload the file given by fileName
        /// Only valid for a few minutes
        /// </summary>
        /// <param name="fileName">file name with extension</param>
        /// <returns></returns>
        Task<SasResponse> GetSharedAccessSignature(string fileName);

        /// <summary>
        /// uploads a number of files to blob storage
        /// </summary>
        Task UploadFiles(params FileUpload[] files);

        string GetFullUriFromIdentifier(string identifier);

        Task<string> UploadFile(IFormFile file);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly AppSettings _appSettings;
        private readonly ConnectionSettings _connectionSetting;

        public FileStorageService(IOptions<ConnectionSettings> connectionSetting, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _connectionSetting = connectionSetting.Value;
        }


        public async Task<SasResponse> GetSharedAccessSignature(string fileName)
        {
            string storageConnectionString = _connectionSetting.StorageConnection;

            // Create the storage account with the connection string.
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client object.
            var blobClient = storageAccount.CreateCloudBlobClient();

            var props = await blobClient.GetServicePropertiesAsync();

            if (!props.Cors.CorsRules.Any())
            {
                //create the default cors access rule
                var cors = new CorsRule();
                cors.AllowedHeaders.Add("*");
                cors.AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Put;
                cors.AllowedOrigins.Add("*"); //TODO: only allow certain origins
                cors.ExposedHeaders.Add("x-ms-*");
                cors.MaxAgeInSeconds = 60 * 60 * 24 * 365; //one year
                props.Cors.CorsRules.Clear();
                props.Cors.CorsRules.Add(cors);
                await blobClient.SetServicePropertiesAsync(props);
            }

            // Get a reference to the container for which shared access signature will be created.
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.StorageContainerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(fileName);
            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(20),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read
            });

            var accessUrl = string.Format("{0}{1}", blob.Uri.AbsoluteUri, sas);
            return new SasResponse { AccessUrl = accessUrl, Url = blob.Uri.AbsoluteUri, Identifier = fileName };
        }

        public async Task UploadFiles(params FileUpload[] files)
        {
            CloudBlobContainer container = await GetContainer();
            foreach (var fileUpload in files)
            {
                var blob = container.GetBlockBlobReference(fileUpload.Identifier);
                blob.Properties.ContentType = fileUpload.ContentType;

                if (!string.IsNullOrWhiteSpace(fileUpload.CacheControl))
                {
                    blob.Properties.CacheControl = fileUpload.CacheControl;
                }

                fileUpload.Data.Seek(0, SeekOrigin.Begin); //go back to beginning of the stream to get all data

                await blob.UploadFromStreamAsync(fileUpload.Data);
                await blob.SetPropertiesAsync();
            }

        }

        private async Task<CloudBlobContainer> GetContainer()
        {
            string storageConnectionString = _connectionSetting.StorageConnection;

            // Create the storage account with the connection string.
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client object
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to the container for which shared access signature will be created.
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.StorageContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public string GetFullUriFromIdentifier(string identifier)
        {
            return $"{_appSettings.StorageUrlBase}/{_appSettings.StorageContainerName}/{identifier}";
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var name = Path.GetFileNameWithoutExtension(file.FileName) ?? "UnknownFileName";
            name = Regex.Replace(name, @"[^\w]", "-"); //replace anything non-word chars with -
            var randomId = Guid.NewGuid().ToString();

            var nameBase = string.Format("{0}-{1}{2}", randomId, name, extension);

            FileUpload fileUpload = new FileUpload();
            fileUpload.ContentType = file.ContentType;
            fileUpload.Identifier = nameBase;
            fileUpload.Data = file.OpenReadStream();


            await UploadFiles(fileUpload);

            return fileUpload.Identifier;
        }
    }
}
