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
    class IncomeViewAdapter : BaseAdapter<IncomeData>
    {
        private List<IncomeData> Items;
        private Context Context;

        public IncomeViewAdapter(Context context, List<IncomeData> items)
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

        public override IncomeData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.IncomeTable, null, false);
            }

            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateIncome = row.FindViewById<TextView>(Resource.Id.txtDateIncome);
            TextView txtIncomeIncome = row.FindViewById<TextView>(Resource.Id.txtIncomeIncome);

            switch (textSize)
            {
                case 0:
                    txtDateIncome.TextSize = 10;
                    txtIncomeIncome.TextSize = 10;
                    break;
                case 1:
                    txtDateIncome.TextSize = 15;
                    txtIncomeIncome.TextSize = 15;
                    break;
                case 2:
                    txtDateIncome.TextSize = 20;
                    txtIncomeIncome.TextSize = 20;
                    break;
            }

            if (NightSwitchMode)
            {
                row.SetBackgroundColor(Color.Black);
                txtDateIncome.SetTextColor(Color.White);
                txtIncomeIncome.SetTextColor(Color.White);
            }
            else
            {
                row.SetBackgroundColor(Color.White);
                txtDateIncome.SetTextColor(Color.Black);
                txtIncomeIncome.SetTextColor(Color.Black);
            }

            if (DateSwitchMode)
            {
                Items.Sort(delegate (IncomeData one, IncomeData two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }

            txtDateIncome.Text = Items[position].GetDate().ToShortDateString();
            txtIncomeIncome.Text = "$ " + Items[position].GetIncome().ToString();

            return row;
        }

    }
}