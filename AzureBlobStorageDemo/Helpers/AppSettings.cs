
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace AzureBlobStorageDemo.Helpers
{
    public static partial class AppSettings
    {
        public static string ConnectionString { get; set; }
        public static string AppUrl { get; set; }
        public static  string FuncUrl { get; set; }

        public delegate void OnInvalidateToken();
        public static async void SetToken(Context context)
        {
            var onInvalidateToken=new OnInvalidateToken(InvalidateToken);

            var client = new HttpClient();
            var result = await client.GetAsync(FuncUrl);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var token=await result.Content.ReadAsStringAsync();
                Preferences.Set("sas_token",token);
                onInvalidateToken.Invoke();
            }
            else
            {
                Toast.MakeText(context, "An error occured", ToastLength.Short).Show();
            }
        }

        private static async void InvalidateToken()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(240000);
                ThreadPool.QueueUserWorkItem(o =>
                {
                    Thread.Sleep(240000);
                    Preferences.Remove("sas_token");
                });
            });
        }
    }
}