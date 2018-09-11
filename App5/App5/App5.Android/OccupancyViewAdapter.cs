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

            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateOccupancy = row.FindViewById<TextView>(Resource.Id.txtDateOccupancy);
            TextView txtOccupancyOccupancy = row.FindViewById<TextView>(Resource.Id.txtOccupancyOccupancy);
            TextView txtConcessionalOccupancy = row.FindViewById<TextView>(Resource.Id.txtConcessionalOccupancy);
            TextView txtCareTypeOccupancy = row.FindViewById<TextView>(Resource.Id.txtCareTypeOccupancy);

            switch (textSize) {
                case 0:
                    txtDateOccupancy.TextSize = 10;
                    txtOccupancyOccupancy.TextSize = 10;
                    txtConcessionalOccupancy.TextSize = 10;
                    txtCareTypeOccupancy.TextSize = 10;
                    break;
                case 1:
                    txtDateOccupancy.TextSize = 15;
                    txtOccupancyOccupancy.TextSize = 15;
                    txtConcessionalOccupancy.TextSize = 15;
                    txtCareTypeOccupancy.TextSize = 15;
                    break;
                case 2:
                    txtDateOccupancy.TextSize = 20;
                    txtOccupancyOccupancy.TextSize = 20;
                    txtConcessionalOccupancy.TextSize = 20;
                    txtCareTypeOccupancy.TextSize = 20;
                    break;
            }

            if (NightSwitchMode) {
                row.SetBackgroundColor(Color.Black);
                txtDateOccupancy.SetTextColor(Color.White);
                txtOccupancyOccupancy.SetTextColor(Color.White);
                txtConcessionalOccupancy.SetTextColor(Color.White);
                txtCareTypeOccupancy.SetTextColor(Color.White);
            } else {
                row.SetBackgroundColor(Color.White);
                txtDateOccupancy.SetTextColor(Color.Black);
                txtOccupancyOccupancy.SetTextColor(Color.Black);
                txtConcessionalOccupancy.SetTextColor(Color.Black);
                txtCareTypeOccupancy.SetTextColor(Color.Black);
            }

            if (DateSwitchMode) {
                Items.Sort(delegate (OccupancyData one, OccupancyData two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }

            txtDateOccupancy.Text = Items[position].GetDate().ToShortDateString();
            txtOccupancyOccupancy.Text = Items[position].GetOccupancy().ToString();
            txtCareTypeOccupancy.Text = Items[position].GetCareType();
            txtConcessionalOccupancy.Text = Items[position].GetConcessional().ToString();

            return row;
        }

    }
}