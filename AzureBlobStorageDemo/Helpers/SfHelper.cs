using Android.Graphics;
using Android.Views;
using Android.Widget;
using Syncfusion.Android.DataForm;

namespace AzureBlobStorageDemo.Helpers
{
    public class SfHelper:DataFormLayoutManager
    {
        private EditText _editText;
        public SfHelper(SfDataForm dataForm) : base(dataForm)
        {

        }

        protected override void OnEditorCreated(DataFormItem dataFormItem, View editor)
        {
            base.OnEditorCreated(dataFormItem, editor);
            if (editor is EditText edtTxt)
                _editText = edtTxt;
            _editText.Typeface = Typeface.Default;
            _editText.SetBackgroundResource(Resource.Drawable.syncfusion_editText_style);
            _editText.SetTextColor(Color.Black);
            _editText.SetHintTextColor(Color.WhiteSmoke);
        }

        protected override View GenerateViewForLabel(DataFormItem dataFormItem)
        {
             var label=base.GenerateViewForLabel(dataFormItem);
            if (label is TextView view)
            {
                view.Typeface = Typeface.Default;
                view.TextSize = 16;
                view.SetTextColor(Color.Black);
            }
            return label;
        }
    }
}