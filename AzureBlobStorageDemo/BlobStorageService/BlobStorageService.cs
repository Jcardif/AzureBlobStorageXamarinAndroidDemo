using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public BlobStorageService()
        {
            _fullResBlobContainer = _blobClient.GetContainerReference("fullres-aeroplane-images");
            _lowResBlobContainer = _blobClient.GetContainerReference("lowres-aeroplane-images");
            _mediumResBlobContainer = _blobClient.GetContainerReference("mediumres-aeroplane-images");

            InitialiseSettings();
        }

        private CloudBlobClient _blobClient = CloudStorageAccount.Parse(ConnectionString).CreateCloudBlobClient();

        public async Task<List<Uri>> GetAllUrisAsync()
        {
            var token=new BlobContinuationToken();
            var allBlobs = await _fullResBlobContainer.ListBlobsSegmentedAsync(token).ConfigureAwait(false);
            return allBlobs.Results.Select(b => b.Uri).ToList();
        }

        public async Task UploadImageAsync(string localPath)
        {
            var blobName = Guid.NewGuid().ToString();
            blobName += Path.GetExtension(localPath);

            var blobRef = _fullResBlobContainer.GetBlockBlobReference(blobName);
            await blobRef.UploadFromFileAsync(localPath).ConfigureAwait(false);
        }
    }
}