
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AzureBlobStorageDemo.Helpers
{
    public static partial class AppSettings
    {
        public static string ConnectionString { get; set; }
        public static string AppUrl { get; set; }
    }
}