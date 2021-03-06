﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AzureBlobStorageDemo.Adapters;
using AzureBlobStorageDemo.AzureMobileService;
using AzureBlobStorageDemo.DialogFragment;
using AzureBlobStorageDemo.Models;
using Plugin.CurrentActivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Square.Picasso;
using Syncfusion.Android.DataForm;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AzureBlobStorageDemo.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ProgressBar progressBar;
        private RecyclerView _recyclerView;
        private MediaFile file;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Picasso.With(this)
                .IndicatorsEnabled = true;

            // This MobileServiceClient has been configured to communicate with the Azure Mobile App and
            // Azure Gateway using the application url. You're all set to start working with your Mobile App!
           
            CrossCurrentActivity.Current.Init(this,savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);

            fab.Click += FabOnClick;
            GetData(AeroplanesService.Loading.fast);
            //GetData(AeroplanesService.Loading.slow);
        }


        private async void GetData(AeroplanesService.Loading l)
        {
            var aeroplaneService = new AeroplanesService();
            var planes = await aeroplaneService.GetAeroplanes(l);
            _recyclerView.SetAdapter(new RecyclerViewAdapter(planes, this));
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
                     GetData(AeroplanesService.Loading.slow);
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
                file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Aeroplanes",
                        Name = $"{DateTime.Now}_Plane.jpg"
                    });
                if (file == null) return;
                //Todo: get aeroplane info
               ShowInfoDialog();

            }
            else if (e.Code == 2)
            {
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    SaveMetaData = true,
                    CompressionQuality = 30
                });
                if(file==null) return;
                ShowInfoDialog();
            }
        }

        private void ShowInfoDialog()
        {
            var dialog = new AeroplaneDetailsDialogFragment();
            dialog.Show(FragmentManager.BeginTransaction(), "TAG2");
            dialog.OnAeroplaneInfoComplete += Dialog_OnAeroplaneInfoComplete;
        }

        private async void Dialog_OnAeroplaneInfoComplete(object sender, AeroplaneInfo e)
        {
            progressBar.Visibility = ViewStates.Visible;
            var uri=await new BlobStorageService().UploadToBlobContainer(file.Path);
            Toast.MakeText(this,"Uploaded to blob storage",ToastLength.Short).Show();
            new File(file.Path).Delete();
            var plane = new Aeroplane
            {
                Name = e.plane.Name,
                Description = e.plane.Description,
                ImageUri = uri.ToString()
            };
            await new AeroplanesService().AddAeroplane(plane);
            Toast.MakeText(this, "Added to database",ToastLength.Short).Show();
            GetData(AeroplanesService.Loading.fast);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}