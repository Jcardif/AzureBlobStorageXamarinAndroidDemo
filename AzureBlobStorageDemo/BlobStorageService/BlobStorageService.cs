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
        private CloudBlobContainer _fullResBlobContainer;
        private CloudBlobContainer _lowResBlobContainer;
        private CloudBlobContainer _mediumResBlobContainer;
        private CloudBlobClient _blobClient;
        private CloudStorageAccount _cloudStorageAccount;
        public BlobStorageService()
        {
            InitialiseSettings();
            _cloudStorageAccount=CloudStorageAccount.Parse(ConnectionString);
            _blobClient = _cloudStorageAccount.CreateCloudBlobClient();

            _fullResBlobContainer = _blobClient.GetContainerReference("fullres-aeroplane-images");
            _lowResBlobContainer = _blobClient.GetContainerReference("lowres-aeroplane-images");
            _mediumResBlobContainer = _blobClient.GetContainerReference("mediumres-aeroplane-images");
        }

        public async Task<Uri> UploadToBlobContainer(string filePath)
        {
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
            var blockBlob = _fullResBlobContainer.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromFileAsync(filePath);
            return _fullResBlobContainer.GetBlobReference(blobName).Uri;
        }
    }
}