using System;
using  Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace AzureBlobStorageDemo.DialogFragment
{
    public class Selection : EventArgs
    {
        public Selection(int code)
        {
            Code = code;
        }

        public int Code { get; set; }
    }
    public class SelectOptionDialogFragment : Android.App.DialogFragment
    {
        private Button _cameraBtn;
        private Button _picBtn;
        public event EventHandler<Selection> OnSelectionComplete;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.layout_alertDialog, container, false);
            _cameraBtn = view.FindViewById<Button>(Resource.Id.takePicCamera);
            _picBtn = view.FindViewById<Button>(Resource.Id.selectPic);

            _cameraBtn.Click += (s, e) => { SelectionComplete(1); };
            _picBtn.Click += (s, e) => { SelectionComplete(2); };

            return view;

        }

        private void SelectionComplete(int code)
        {
            switch (code)
            {
                case 1:
                    OnSelectionComplete?.Invoke(this, new Selection(code));
                    Dismiss();
                    break;
                case 2:
                    OnSelectionComplete?.Invoke(this, new Selection(code));
                    Dismiss();
                    break;
                default:
                    break;
            }
        }


        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //set the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
        }
    }
}