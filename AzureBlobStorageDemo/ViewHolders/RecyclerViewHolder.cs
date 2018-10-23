using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
namespace AzureBlobStorageDemo.ViewHolders
{
    public class RecyclerViewHolder:RecyclerView.ViewHolder
    {
        public RecyclerViewHolder(View view) : base(view)
        {
            View = view;
        }

        public View View { get; set; }
        public ImageView ImageView { get; set; }
        public TextView TitleTextView { get; set; }
    }
}