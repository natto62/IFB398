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
    class StaffViewAdapter : BaseAdapter<StaffData>
    {

        private List<StaffData> Items;
        private Context Context;

        public StaffViewAdapter(Context context, List<StaffData> items)
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

        public override StaffData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.StaffTable, null, false);
            }

            TextView txtFNameStaff = row.FindViewById<TextView>(Resource.Id.txtFNameStaff);
            txtFNameStaff.Text = Items[position].GetStaffFirstName();

            TextView txtLNameStaff = row.FindViewById<TextView>(Resource.Id.txtLNameStaff);
            txtLNameStaff.Text = Items[position].GetStaffLastName();

            TextView txtAnnualLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtAnnualLeaveStaff);
            txtAnnualLeaveStaff.Text = "$ " + Items[position].GetAnnualLeaveAcrewed().ToString();

            TextView txtLongServiceLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtLongServiceLeaveStaff);
            txtLongServiceLeaveStaff.Text = "$ " + Items[position].GetLongServiceLeaveAcrewed().ToString();

            TextView txtServiceLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtServiceLeaveStaff);
            txtServiceLeaveStaff.Text = "$ " + Items[position].GetServiceLeaveAcrewed().ToString();

            return row;
        }

    }
}