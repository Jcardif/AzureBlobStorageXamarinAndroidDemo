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
using AzureBlobStorageDemo.Models;
using AzureBlobStorageDemo.ViewHolders;
using Square.Picasso;

namespace AzureBlobStorageDemo.Adapters
{
    public class RecyclerViewAdapter: RecyclerView.Adapter, ICallback
    {
        private List<Aeroplane> _aeroplanes;
        private Context _context;
        private RecyclerView.ViewHolder _holder;
        private int _position;
        public RecyclerViewAdapter(List<Aeroplane> aeroplanes, Context context)
        {
            _aeroplanes = aeroplanes;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is RecyclerViewHolder vh)) return;

            _holder = vh;
            _position = position;

            CustomBindViewHolder(vh, position);
        }

        private void CustomBindViewHolder(RecyclerViewHolder vh, int position)
        {
            vh.TitleTextView.Text = _aeroplanes[position].Name;

            Picasso.With(_context)
                .IndicatorsEnabled = true;

            Picasso.With(_context)
                .Load(_aeroplanes[position].Uri)

                .Into(vh.ImageView,this);
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(_context).Inflate(Resource.Layout.aeroplane_Item, parent, false);
             var imgView = view.FindViewById<ImageView>(Resource.Id.aero_ImgView);
            var textView = view.FindViewById<TextView>(Resource.Id.nameTxtView);
            var viewHolder = new RecyclerViewHolder(view)
            {
                ImageView = imgView,
                TitleTextView = textView
            };

            return viewHolder;
        }

        public override int ItemCount => _aeroplanes.Count;
        public void OnError()
        {
            try
            {
                Picasso.With(_context)
                    .Load(_aeroplanes[_position].Uri)
                    .Fetch();
            }
            catch (Exception ex)
            {
                Toast.MakeText(_context, ex.Message, ToastLength.Short).Show();
            }
        }

        public void OnSuccess()
        {

        }
    }
}