using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AzureBlobStorageDemo.Adapters;
using AzureBlobStorageDemo.DialogFragment;
using AzureBlobStorageDemo.Models;
using Plugin.CurrentActivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AzureBlobStorageDemo.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ProgressBar progressBar;
        private RecyclerView _recyclerView;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this,savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            
            fab.Click += FabOnClick;
            GetData();
        }


        private async void GetData()
        {
            var aeroplanes = new List<Aeroplane>();
            var uris = await new BlobStorageService().GetAllUrisAsync();
            foreach (var a in uris)
            {
                aeroplanes.Add(new Aeroplane { Title = "1", Uri = a.ToString() });
            }
            _recyclerView.SetAdapter(new RecyclerViewAdapter(aeroplanes, this));
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            progressBar.Visibility = ViewStates.Invisible;
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            switch (id)
            {
                case Resource.Id.action_settings:
                    return true;
                case Resource.Id.action_refresh:
                    progressBar.Visibility = ViewStates.Visible;
                     GetData();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private async void FabOnClick(object sender, EventArgs eventArgs)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this,"Device Not Supported", ToastLength.Long).Show();
            }
            else
            {
                var selectOptionDialogFragment = new SelectOptionDialogFragment();
                selectOptionDialogFragment.Show(FragmentManager.BeginTransaction(), "TAG");
                selectOptionDialogFragment.OnSelectionComplete += SelectOptionDialogFragment_OnSelectionComplete;
            }
        }

        private async void SelectOptionDialogFragment_OnSelectionComplete(object sender, Selection e)
        {
            if (e.Code == 1)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Aeroplanes",
                        Name = $"{DateTime.Now}_Plane.jpg"
                    });
                if (file == null) return;
                progressBar.Visibility = ViewStates.Visible;
                await new BlobStorageService().UploadImageAsync(file.Path);
                file.Dispose();
                 GetData();
            }
            else if (e.Code == 2)
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    SaveMetaData = true,
                    CompressionQuality = 30
                });
                progressBar.Visibility = ViewStates.Visible;
                await new BlobStorageService().UploadImageAsync(file.Path);
                file.Dispose();
                GetData();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}