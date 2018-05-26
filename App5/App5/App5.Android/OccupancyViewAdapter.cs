using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MiCareApp.Droid
{
    class OccupancyViewAdapter : BaseAdapter<OccupancyData>
    {

        private List<OccupancyData> Items;
        private Context Context;

        public OccupancyViewAdapter(Context context, List<OccupancyData> items)
        {
            Items = items;
            Context = context;
        }

        //count items, create that number of rows
        public override int Count
        {
            get { return Items.Count; }
        }

        //return item id
        public override long GetItemId(int position)
        {
            return position;
        }

        public override OccupancyData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.OccupancyTable, null, false);
            }

            TextView txtDateOccupancy = row.FindViewById<TextView>(Resource.Id.txtDateOccupancy);
            txtDateOccupancy.Text = Items[position].GetDate().ToShortDateString();

            TextView txtOccupancyOccupancy = row.FindViewById<TextView>(Resource.Id.txtOccupancyOccupancy);
            txtOccupancyOccupancy.Text = Items[position].GetOccupancy().ToString();

            TextView txtConcessionalOccupancy = row.FindViewById<TextView>(Resource.Id.txtConcessionalOccupancy);
            txtConcessionalOccupancy.Text = "$ " + Items[position].GetConcessional().ToString();

            TextView txtVacancyOccupancy = row.FindViewById<TextView>(Resource.Id.txtVacancyOccupancy);
            txtVacancyOccupancy.Text = " ";
            //^^^^^^
            //vacancy

            return row;
        }

    }
}