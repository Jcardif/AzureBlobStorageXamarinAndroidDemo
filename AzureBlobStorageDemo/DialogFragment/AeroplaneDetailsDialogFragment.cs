using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using AzureBlobStorageDemo.Models;
using Plugin.CurrentActivity;
using Syncfusion.Android.DataForm;

namespace AzureBlobStorageDemo.DialogFragment
{
    public class AeroplaneInfo : EventArgs
    {
        public Aeroplane plane;
        public AeroplaneInfo(Aeroplane plane)
        {
            this.plane = plane;
        }
    }
    public class AeroplaneDetailsDialogFragment : Android.App.DialogFragment
    {
        Aeroplane _plane=new Aeroplane();
        public event EventHandler<AeroplaneInfo> OnAeroplaneInfoComplete; 
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var dataformParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent);
            dataformParams.Height = ViewGroup.LayoutParams.WrapContent;
            dataformParams.Width = ViewGroup.LayoutParams.WrapContent;


            var sfDataform = new SfDataForm(Context.ApplicationContext);
            sfDataform.DataObject = _plane;
            sfDataform.LabelPosition = LabelPosition.Top;
            sfDataform.ValidationMode = ValidationMode.LostFocus;
            sfDataform.CommitMode = CommitMode.LostFocus;
            sfDataform.Id = View.GenerateViewId();

            var buttonParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent);
            buttonParams.AddRule(LayoutRules.Below, sfDataform.Id);
            buttonParams.Height = ViewGroup.LayoutParams.WrapContent;
            buttonParams.Width = ViewGroup.LayoutParams.WrapContent;
            buttonParams.AddRule(LayoutRules.CenterHorizontal);

            var button = new Button(Context.ApplicationContext)
            {
                Text = "Done",
                Id = View.GenerateViewId()
            };

            var view = new RelativeLayout(CrossCurrentActivity.Current.Activity);
            view.AddView(sfDataform, dataformParams);
            view.AddView(button, buttonParams);


            button.Click +=  (s, e) =>
            {
                sfDataform.Validate();
                sfDataform.Commit();
                if (sfDataform.Validate())
                {
                    OnAeroplaneInfoComplete?.Invoke(this, new AeroplaneInfo(_plane));
                }
            };
            return view;
        }
    }
}