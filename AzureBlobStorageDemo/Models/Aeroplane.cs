using Android.Net;
using Newtonsoft.Json;
using  Microsoft.WindowsAzure.MobileServices;

namespace AzureBlobStorageDemo.Models
{
    public class Aeroplane
    {
        public string Id { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        [JsonIgnore]
        public string UriLoad { get; set; }
        [Version]
        public string AzureVersion { get; set; }

        [CreatedAt]
        public string AzureCreated { get; set; }

        [UpdatedAt]
        public string AzureUpdated { get; set; }

        [Deleted]
        public string AzureDeleted { get; set; }
    }
}