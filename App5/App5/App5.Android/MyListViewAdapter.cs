using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App5.Droid;

namespace MiCareApp.Droid 
{
    class MyListViewAdapter : BaseAdapter<FinanceData>
    {
        private List<FinanceData> Items;
        private Context ThisContext;

        public MyListViewAdapter(Context context, List<FinanceData> items)
        {
            Items = items;
            ThisContext = context;
        }

        //count items, create that number of rows
        public override int Count {
            get { return Items.Count; }
        }

        //return item id
        public override long GetItemId(int position)
        {
            return position;
        }

        public override FinanceData this[int position] {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null) {
                row = LayoutInflater.From(ThisContext).Inflate(Resource.Layout.DataPage, null, false);
            }

            TextView txtFName = row.FindViewById<TextView>(Resource.Id.txtFName);
            txtFName.Text = Items[position].GetFirstName();

            TextView txtLName = row.FindViewById<TextView>(Resource.Id.txtLName);
            txtLName.Text = Items[position].GetLastName();

            TextView txtIncome = row.FindViewById<TextView>(Resource.Id.txtIncome);
            txtIncome.Text = "$ " + Items[position].GetIncome();

            TextView txtStatus = row.FindViewById<TextView>(Resource.Id.txtStatus);
            //txtStatus.SetBackgroundResource(Resource.Drawable.Corners);
            if (Items[position].IsGreen())
            {
                txtStatus.SetBackgroundColor(Color.Green);
            } else if (Items[position].IsRed())
            {
                txtStatus.SetBackgroundColor(Color.Red);
            } else
            {
                txtStatus.SetBackgroundColor(Color.Yellow);
            }
            
            txtStatus.Text = " ";
            

            return row;
        }

    }
}