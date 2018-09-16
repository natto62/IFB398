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

namespace MiCareApp.Droid
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

            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateBank = row.FindViewById<TextView>(Resource.Id.txtDateBank);
            TextView txtBalanceBank = row.FindViewById<TextView>(Resource.Id.txtBalanceBank);

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
                row.SetBackgroundColor(Color.Black);
                txtDateBank.SetTextColor(Color.White);
                txtBalanceBank.SetTextColor(Color.White);
            }
            else
            {
                row.SetBackgroundColor(Color.White);
                txtDateBank.SetTextColor(Color.Black);
                txtBalanceBank.SetTextColor(Color.Black);
            }

            if (DateSwitchMode)
            {
                Items.Sort(delegate (BankBalance one, BankBalance two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }

            txtDateBank.Text = Items[position].GetDate().ToShortDateString();
            txtBalanceBank.Text = "$ " + Items[position].GetBankBalance().ToString();

            return row;
        }

    }
}