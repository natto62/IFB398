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
using Android;

namespace MiCareDBapp.Droid
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

            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateBrokerage = row.FindViewById<TextView>(Resource.Id.txtDateBrokerage);
            TextView txtHoursBrokerage = row.FindViewById<TextView>(Resource.Id.txtHoursBrokerage);

            switch (textSize) {
                case 0:
                    txtDateBrokerage.TextSize = 10;
                    txtHoursBrokerage.TextSize = 10;
                    break;
                case 1:
                    txtDateBrokerage.TextSize = 15;
                    txtHoursBrokerage.TextSize = 15;
                    break;
                case 2:
                    txtDateBrokerage.TextSize = 20;
                    txtHoursBrokerage.TextSize = 20;
                    break;
            }

            if (NightSwitchMode) {
                txtDateBrokerage.SetTextColor(Color.White);
                txtHoursBrokerage.SetTextColor(Color.White);
            } else {
                txtDateBrokerage.SetTextColor(Color.Black);
                txtHoursBrokerage.SetTextColor(Color.Black);
            }

            if (DateSwitchMode) {
                Items.Sort(delegate (BrokerageHoursData one, BrokerageHoursData two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }

            txtDateBrokerage.Text = Items[position].GetDate().ToShortDateString();
            txtHoursBrokerage.Text = Items[position].GetBrokerageHours().ToString();

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