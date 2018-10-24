using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Provider;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using  static AzureBlobStorageDemo.Helpers.AppSettings;

namespace AzureBlobStorageDemo
{
    public class BlobStorageService
    {
        private CloudBlobContainer _blobContainer;
        private CloudBlobClient _blobClient;
        private CloudStorageAccount _cloudStorageAccount;
        public BlobStorageService()
        {
            InitialiseSettings();
            _cloudStorageAccount=CloudStorageAccount.Parse(ConnectionString);
            _blobClient = _cloudStorageAccount.CreateCloudBlobClient();

            _blobContainer = _blobClient.GetContainerReference("aeroplane-images");
        }

        public async Task<Uri> UploadToBlobContainer(string filePath)
        {
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
            var blockBlob = _blobContainer.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromFileAsync(filePath);
            return _blobContainer.GetBlobReference(blobName).Uri;
        }
    }
}