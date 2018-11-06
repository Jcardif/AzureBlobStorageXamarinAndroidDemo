using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using AzureBlobStorageDemo.Helpers;
using AzureBlobStorageDemo.Models;
using AzureBlobStorageDemo.ViewHolders;
using Plugin.Connectivity;
using Square.Picasso;
using Xamarin.Essentials;
using static AzureBlobStorageDemo.Helpers.AppSettings;

namespace AzureBlobStorageDemo.Adapters
{
    public class RecyclerViewAdapter: RecyclerView.Adapter, ICallback
    {
        private List<Aeroplane> _aeroplanes;
        private Context _context;
        private RecyclerViewHolder _holder;
        private int _position;
        private ImageView _imgView;
        private TextView _txtViewdesc;
        
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

        private async void CustomBindViewHolder(RecyclerViewHolder vh, int position)
        {
            vh.TitleTextView.Text = _aeroplanes[position].Name;
            vh.DescTextView.Text = _aeroplanes[position].Description;

            if (Preferences.Get("sas_token",null) is null)
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Toast.MakeText(_context,"No internet connection", ToastLength.Short).Show();
                    return;
                }

                var token = Preferences.Get("sas_token", null);
                if (token is null) SetToken(_context);
                Picasso.With(_context)
                    .Load(_aeroplanes[_position].ImageUri+token)
                    .Fetch(this);
            }
             
            Picasso.With(_context)
                .Load(_aeroplanes[position].ImageUri+ Preferences.Get("sas_token", null))
                .Fit()
                .CenterCrop()
                .NetworkPolicy(NetworkPolicy.Offline)
                .Into(vh.ImageView, this);
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Picasso.With(_context)
                .IndicatorsEnabled = true;

            var view = LayoutInflater.From(_context).Inflate(Resource.Layout.aeroplane_Item, parent, false);
            _imgView = view.FindViewById<ImageView>(Resource.Id.aero_ImgView);
            var textView = view.FindViewById<TextView>(Resource.Id.nameTxtView);
            _txtViewdesc = view.FindViewById<TextView>(Resource.Id.descTxtView);
            

            var viewHolder = new RecyclerViewHolder(view)
            {
                ImageView = _imgView,
                TitleTextView = textView,
                DescTextView = _txtViewdesc
            };

            return viewHolder;
        }

        public override int ItemCount => _aeroplanes.Count;
        public async void OnError()
        {
            try
            {
                Toast.MakeText(_context, $"An error occured for {_aeroplanes[_position].Name}", ToastLength.Short).Show();
                var token = Preferences.Get("sas_token", null);
                if (token is null) SetToken(_context);
                Picasso.With(_context)
                    .Load(_aeroplanes[_position].ImageUri+token)
                    .Fetch(this);
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