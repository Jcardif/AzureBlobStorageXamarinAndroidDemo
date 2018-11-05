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
using AzureBlobStorageDemo.AzureMobileService;
using AzureBlobStorageDemo.DialogFragment;
using AzureBlobStorageDemo.Models;
using Plugin.CurrentActivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Syncfusion.Android.DataForm;
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
            GetData();
        }


        private async void GetData()
        {
            var aeroplaneService = new AeroplanesService();
            var planes = await aeroplaneService.GetAeroplanes();
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
                file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Aeroplanes",
                        Name = $"{DateTime.Now}_Plane.jpg"
                    });
                if (file == null) return;
                //Todo: get aeroplan info
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
            file.Dispose();
            var plane = new Aeroplane
            {
                Name = e.plane.Name,
                Description = e.plane.Description,
                Uri = uri.ToString()
            };
            var plane2 =await new AeroplanesService().AddAeroplane(plane);
            GetData();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}