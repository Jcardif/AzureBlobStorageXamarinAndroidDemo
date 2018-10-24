using Android.Net;
using Newtonsoft.Json;
using  Microsoft.WindowsAzure.MobileServices;
using Syncfusion.Android.DataForm;
using System.ComponentModel.DataAnnotations;

namespace AzureBlobStorageDemo.Models
{
    public class Aeroplane
    {
        [Display(AutoGenerateField = false)]
        public string Id { get; set; }
        [Display(AutoGenerateField = false)]
        public string Uri { get; set; }
        [Display(Name = "Name"), Required(AllowEmptyStrings = false, ErrorMessage = "Name Cannot be Empty")]
        public string Name { get; set; }
        [Display(Name = "Description"), Required(AllowEmptyStrings = false, ErrorMessage = "Description Cannot be Empty")]
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