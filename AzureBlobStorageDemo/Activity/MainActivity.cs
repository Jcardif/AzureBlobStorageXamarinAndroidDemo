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
        private Aeroplane _plane;

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
            
            _plane=new Aeroplane();

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
                var file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Aeroplanes",
                        Name = $"{DateTime.Now}_Plane.jpg"
                    });
                if (file == null) return;


                var dataformParams=new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                dataformParams.Height = ViewGroup.LayoutParams.WrapContent;
                dataformParams.Width = ViewGroup.LayoutParams.WrapContent;
                

                var sfDataform = new SfDataForm(this);
                sfDataform.DataObject = _plane;
                sfDataform.LabelPosition = LabelPosition.Top;
                sfDataform.ValidationMode = ValidationMode.LostFocus;
                sfDataform.CommitMode = CommitMode.LostFocus;
                sfDataform.Id = View.GenerateViewId();

                var buttonParams=new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                buttonParams.AddRule(LayoutRules.Below,sfDataform.Id);
                buttonParams.Height = ViewGroup.LayoutParams.WrapContent;
                buttonParams.Width = ViewGroup.LayoutParams.WrapContent;
                buttonParams.AddRule(LayoutRules.CenterHorizontal);

                var button=new Button(this);
                button.Text="Done";
                button.Id = View.GenerateViewId();

                var relativeLayout = new RelativeLayout(this);
                relativeLayout.AddView(sfDataform, dataformParams);
                relativeLayout.AddView(button, buttonParams);


                var builder = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetView(relativeLayout)
                    .Create();
                 builder.SetCanceledOnTouchOutside(false);
                builder.Show();
                builder.Window.SetLayout(1000, 500);

                button.Click += async (s, ev) =>
                {
                    sfDataform.Validate();
                    sfDataform.Commit();
                    Dispose();

                    progressBar.Visibility = ViewStates.Visible;
                    await new BlobStorageService().UploadToBlobContainer(file.Path);
                    file.Dispose();
                    GetData();
                };

            }
            else if (e.Code == 2)
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    SaveMetaData = true,
                    CompressionQuality = 30
                });
                progressBar.Visibility = ViewStates.Visible;
                await new BlobStorageService().UploadToBlobContainer(file.Path);
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