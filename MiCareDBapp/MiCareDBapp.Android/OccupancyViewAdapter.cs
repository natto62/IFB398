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

            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateOccupancy = row.FindViewById<TextView>(Resource.Id.txtDateOccupancy);
            TextView txtActualBedsOccupancy = row.FindViewById<TextView>(Resource.Id.txtActualBedsOccupancy);
            TextView txtSupportedOccupancy = row.FindViewById<TextView>(Resource.Id.txtSupportedOccupancy);
            TextView txtOccupancyRateOccupancy = row.FindViewById<TextView>(Resource.Id.txtOccupancyRateOccupancy);
            TextView txtBedDaysOccupancy = row.FindViewById<TextView>(Resource.Id.txtBedDaysOccupancy);

            switch (textSize) {
                case 0:
                    txtDateOccupancy.TextSize = 10;
                    txtActualBedsOccupancy.TextSize = 10;
                    txtSupportedOccupancy.TextSize = 10;
                    txtOccupancyRateOccupancy.TextSize = 10;
                    txtBedDaysOccupancy.TextSize = 10;
                    break;
                case 1:
                    txtDateOccupancy.TextSize = 15;
                    txtActualBedsOccupancy.TextSize = 15;
                    txtSupportedOccupancy.TextSize = 15;
                    txtOccupancyRateOccupancy.TextSize = 15;
                    txtBedDaysOccupancy.TextSize = 15;
                    break;
                case 2:
                    txtDateOccupancy.TextSize = 20;
                    txtActualBedsOccupancy.TextSize = 20;
                    txtSupportedOccupancy.TextSize = 20;
                    txtOccupancyRateOccupancy.TextSize = 20;
                    txtBedDaysOccupancy.TextSize = 20;
                    break;
            }

            if (NightSwitchMode) {
                row.SetBackgroundColor(Color.Black);
                txtDateOccupancy.SetTextColor(Color.White);
                txtActualBedsOccupancy.SetTextColor(Color.White);
                txtSupportedOccupancy.SetTextColor(Color.White);
                txtOccupancyRateOccupancy.SetTextColor(Color.White);
                txtBedDaysOccupancy.SetTextColor(Color.White);
            } else {
                row.SetBackgroundColor(Color.White);
                txtDateOccupancy.SetTextColor(Color.Black);
                txtActualBedsOccupancy.SetTextColor(Color.Black);
                txtSupportedOccupancy.SetTextColor(Color.Black);
                txtOccupancyRateOccupancy.SetTextColor(Color.Black);
                txtBedDaysOccupancy.SetTextColor(Color.Black);
            }

            if (DateSwitchMode) {
                Items.Sort(delegate (OccupancyData one, OccupancyData two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }
            //fix formatting later
            txtDateOccupancy.Text = Items[position].GetDate().ToShortDateString();
            txtActualBedsOccupancy.Text = Items[position].GetActualBeds().ToString();
            txtOccupancyRateOccupancy.Text = Items[position].GetOccupancyRate().ToString();
            txtSupportedOccupancy.Text = Items[position].GetSupported().ToString();
            txtBedDaysOccupancy.Text = Items[position].GetTotalBedDays().ToString();

            return row;
        }

    }
}