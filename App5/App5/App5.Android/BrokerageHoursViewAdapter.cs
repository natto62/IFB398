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
    class BrokerageHoursViewAdapter : BaseAdapter<BrokerageHoursData>
    {

        private List<BrokerageHoursData> Items;
        private Context Context;

        public BrokerageHoursViewAdapter(Context context, List<BrokerageHoursData> items)
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

        public override BrokerageHoursData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.BrokerageHoursTable, null, false);
            }

            TextView txtDateBrokerage = row.FindViewById<TextView>(Resource.Id.txtDateBrokerage);
            txtDateBrokerage.Text = Items[position].GetDate().ToShortDateString();

            TextView txtHoursBrokerage = row.FindViewById<TextView>(Resource.Id.txtHoursBrokerage);
            txtHoursBrokerage.Text = Items[position].GetBrokerageHours().ToString();


            return row;
        }

    }
}