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
using Android;
using System.Globalization;

namespace MiCareDBapp.Droid
{
    class ACFIViewAdapter : BaseAdapter<ACFIFunding>
    {
        private List<ACFIFunding> Items;
        private Context Context;

        public ACFIViewAdapter(Context context, List<ACFIFunding> items)
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

        public override ACFIFunding this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.ACFITable, null, false);
            }

            //retrieve the shared preferences to edit the row attributes such as text size, colour or if the the data has to be sorted by date
            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtResidentACFI = row.FindViewById<TextView>(Resource.Id.txtResidentIDACFI);
            TextView txtScoreACFI = row.FindViewById<TextView>(Resource.Id.txtScoreACFI);
            TextView txtIncomeACFI = row.FindViewById<TextView>(Resource.Id.txtIncomeACFI);
            TextView txtExpirationDateACFI = row.FindViewById<TextView>(Resource.Id.txtExpirationDateACFI);

            switch (textSize) {
                case 0:
                    txtResidentACFI.TextSize = 10;
                    txtScoreACFI.TextSize = 10;
                    txtIncomeACFI.TextSize = 10;
                    txtExpirationDateACFI.TextSize = 10;
                    break;
                case 1:
                    txtResidentACFI.TextSize = 15;
                    txtScoreACFI.TextSize = 15;
                    txtIncomeACFI.TextSize = 15;
                    txtExpirationDateACFI.TextSize = 15;
                    break;
                case 2:
                    txtResidentACFI.TextSize = 20;
                    txtScoreACFI.TextSize = 20;
                    txtIncomeACFI.TextSize = 20;
                    txtExpirationDateACFI.TextSize = 20;
                    break;
            }

            if (NightSwitchMode)
            {
                txtResidentACFI.SetTextColor(Color.White);
                txtScoreACFI.SetTextColor(Color.White);
                txtIncomeACFI.SetTextColor(Color.White);
                txtExpirationDateACFI.SetTextColor(Color.White);
            }
            else
            {
                txtResidentACFI.SetTextColor(Color.Black);
                txtScoreACFI.SetTextColor(Color.Black);
                txtIncomeACFI.SetTextColor(Color.Black);
                txtExpirationDateACFI.SetTextColor(Color.Black);
            }
            //for every item
            txtResidentACFI.Text = Items[position].GetResidentID().ToString();
            txtScoreACFI.Text = Items[position].GetACFIScore().ToString();
            txtIncomeACFI.Text = "$ " + Items[position].GetIncome().ToString("#,#", CultureInfo.InvariantCulture);
            txtExpirationDateACFI.Text = Items[position].GetExpirationDate().ToShortDateString();
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