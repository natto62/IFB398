using System;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Globalization;
using Android;

namespace MiCareDBapp.Droid
{
    class BankViewAdapter : BaseAdapter<BankBalance>
    {
        private List<BankBalance> Items;
        private Context Context;

        public BankViewAdapter(Context context, List<BankBalance> items)
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

        public override BankBalance this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.BankTable, null, false);
            }
            //retrieve the shared preferences to edit the row attributes such as text size, colour or if the the data has to be sorted by date
            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtBalanceBank = row.FindViewById<TextView>(Resource.Id.txtBalanceBank);
            TextView txtDateBank = row.FindViewById<TextView>(Resource.Id.txtDateBank);

            switch (textSize)
            {
                case 0:
                    txtDateBank.TextSize = 10;
                    txtBalanceBank.TextSize = 10;
                    break;
                case 1:
                    txtDateBank.TextSize = 15;
                    txtBalanceBank.TextSize = 15;
                    break;
                case 2:
                    txtDateBank.TextSize = 20;
                    txtBalanceBank.TextSize = 20;
                    break;
            }

            if (NightSwitchMode)
            {
                txtDateBank.SetTextColor(Color.White);
                txtBalanceBank.SetTextColor(Color.White);
            }
            else
            {
                txtDateBank.SetTextColor(Color.Black);
                txtBalanceBank.SetTextColor(Color.Black);
            }

            if (DateSwitchMode)
            {
                Items.Sort(delegate (BankBalance one, BankBalance two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }
            //for every item
            txtDateBank.Text = Items[position].GetDate().ToShortDateString();
            txtBalanceBank.Text = "$ " + Items[position].GetBankBalance().ToString("#,#", CultureInfo.InvariantCulture);
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