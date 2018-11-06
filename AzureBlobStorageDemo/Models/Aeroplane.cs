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
        public string ImageUri { get; set; }
        [Display(Name = "Name"), Required(AllowEmptyStrings = false, ErrorMessage = "Name Cannot be Empty")]
        public string Name { get; set; }
        [Display(Name = "Description"), Required(AllowEmptyStrings = false, ErrorMessage = "Description Cannot be Empty")]
        public string Description { get; set; }
        [JsonIgnore, Display(AutoGenerateField = false)]
        public string LocalImgUrl { get; set; }
        [Version, Display(AutoGenerateField = false)]
        public string AzureVersion { get; set; }

        [CreatedAt, Display(AutoGenerateField = false)]
        public string AzureCreated { get; set; }

        [UpdatedAt, Display(AutoGenerateField = false)]
        public string AzureUpdated { get; set; }

        [Deleted,Display(AutoGenerateField = false)]
        public string AzureDeleted { get; set; }
    }
}