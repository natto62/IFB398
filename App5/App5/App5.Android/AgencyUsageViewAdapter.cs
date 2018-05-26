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
    class AgencyUsageViewAdapter : BaseAdapter<AgencyUsageData>
    {
        private List<AgencyUsageData> Items;
        private Context Context;

        public AgencyUsageViewAdapter(Context context, List<AgencyUsageData> items)
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

        public override AgencyUsageData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.AgencyUsageTable, null, false);
            }

            TextView txtDateAgency = row.FindViewById<TextView>(Resource.Id.txtDateAgency);
            txtDateAgency.Text = Items[position].GetDate().ToShortDateString();

            TextView txtInvoiceIDAgency = row.FindViewById<TextView>(Resource.Id.txtInvoiceIDAgency);
            txtInvoiceIDAgency.Text = Items[position].GetInvoiceID().ToString();

            TextView txtAmountAgency = row.FindViewById<TextView>(Resource.Id.txtAmountAgency);
            txtAmountAgency.Text = "$ " + Items[position].GetAgencyUsageAmount().ToString();

            return row;
        }

    }
}