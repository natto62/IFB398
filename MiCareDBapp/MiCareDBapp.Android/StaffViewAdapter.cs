using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MiCareDBapp.Droid
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
            //retrieve the shared preferences to edit the row attributes such as text size, colour or if the the data has to be sorted by date
            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);

            TextView txtFNameStaff = row.FindViewById<TextView>(Resource.Id.txtFNameStaff);
            TextView txtLNameStaff = row.FindViewById<TextView>(Resource.Id.txtLNameStaff);
            TextView txtAnnualLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtAnnualLeaveStaff);
            TextView txtLongServiceLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtLongServiceLeaveStaff);
            TextView txtSickLeaveStaff = row.FindViewById<TextView>(Resource.Id.txtSickLeaveStaff);

            switch (textSize)
            {
                case 0:
                    txtFNameStaff.TextSize = 10;
                    txtLNameStaff.TextSize = 10;
                    txtAnnualLeaveStaff.TextSize = 10;
                    txtLongServiceLeaveStaff.TextSize = 10;
                    txtSickLeaveStaff.TextSize = 10;
                    break;
                case 1:
                    txtFNameStaff.TextSize = 15;
                    txtLNameStaff.TextSize = 15;
                    txtAnnualLeaveStaff.TextSize = 15;
                    txtLongServiceLeaveStaff.TextSize = 15;
                    txtSickLeaveStaff.TextSize = 15;
                    break;
                case 2:
                    txtFNameStaff.TextSize = 20;
                    txtLNameStaff.TextSize = 20;
                    txtAnnualLeaveStaff.TextSize = 20;
                    txtLongServiceLeaveStaff.TextSize = 20;
                    txtSickLeaveStaff.TextSize = 20;
                    break;
            }

            if (NightSwitchMode)
            {
                txtFNameStaff.SetTextColor(Color.White);
                txtLNameStaff.SetTextColor(Color.White);
                txtAnnualLeaveStaff.SetTextColor(Color.White);
                txtLongServiceLeaveStaff.SetTextColor(Color.White);
                txtSickLeaveStaff.SetTextColor(Color.White);
            }
            else
            {
                txtFNameStaff.SetTextColor(Color.Black);
                txtLNameStaff.SetTextColor(Color.Black);
                txtAnnualLeaveStaff.SetTextColor(Color.Black);
                txtLongServiceLeaveStaff.SetTextColor(Color.Black);
                txtSickLeaveStaff.SetTextColor(Color.Black);
            }
            //for every item
            txtFNameStaff.Text = Items[position].GetStaffFirstName();
            txtLNameStaff.Text = Items[position].GetStaffLastName();
            txtAnnualLeaveStaff.Text = Items[position].GetAnnualLeaveAcrewed().ToString();
            txtLongServiceLeaveStaff.Text = Items[position].GetLongServiceLeaveAcrewed().ToString();
            txtSickLeaveStaff.Text = Items[position].GetSickLeaveAcrewed().ToString();

            //set indicator colours
            if (Items[position].IsGreen())
            {
                if (NightSwitchMode)
                {
                    row.SetBackgroundColor(Color.DarkGreen);
                }
                else
                {
                    row.SetBackgroundColor(Color.LightGreen);
                }
            }
            else if (Items[position].IsRed())
            {
                if (NightSwitchMode)
                {
                    row.SetBackgroundColor(Color.DarkRed);
                }
                else
                {
                    row.SetBackgroundColor(Color.Argb(80, 255, 128, 128));
                }
            }
            else
            {
                if (NightSwitchMode)
                {
                    row.SetBackgroundColor(Color.DarkOrange);
                }
                else
                {
                    row.SetBackgroundColor(Color.LightYellow);
                }
            }

            return row;
        }

    }
}