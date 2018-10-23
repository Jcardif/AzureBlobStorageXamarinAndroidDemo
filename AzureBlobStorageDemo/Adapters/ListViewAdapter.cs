using System.Collections.Generic;
using Android.Content;
using Android.Net;
using Android.Views;
using Android.Widget;
using AzureBlobStorageDemo.Models;
using Java.Lang;
using Refractored.Controls;
using Square.Picasso;

namespace AzureBlobStorageDemo.Adapters
{
    public class ListViewAdapter : BaseAdapter
    {
        private readonly List<Aeroplane> _aeroplanes;
        private readonly Context _context;

        public ListViewAdapter(Context context, List<Aeroplane> aeroplanes)
        {
            _context = context;
            _aeroplanes = aeroplanes;
        }

        public override int Count => _aeroplanes.Count;

        public override Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater) _context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.aeroplane_Item, parent, false);

            var circleImgView = view.FindViewById<CircleImageView>(Resource.Id.circleImgView);
            var titleTxtView = view.FindViewById<TextView>(Resource.Id.nameTxtView);

            titleTxtView.Text = _aeroplanes[position].Title;
            Picasso.With(_context)
                .Load(_aeroplanes[position].Uri)
                .Into(circleImgView);


            return view;
        }
    }
}