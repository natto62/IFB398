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
    class HomeCarePackageViewAdapter : BaseAdapter<HomeCarePackageData>
    {
        private List<HomeCarePackageData> Items;
        private Context Context;

        public HomeCarePackageViewAdapter(Context context, List<HomeCarePackageData> items) {
            Items = items;
            Context = context;
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

        public override HomeCarePackageData this[int position] {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null) {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.HomeCarePackageTable, null, false);
            }
            //retrieve the shared preferences to edit the row attributes such as text size, colour or if the the data has to be sorted by date
            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);

            TextView txtFNameHomeCare = row.FindViewById<TextView>(Resource.Id.txtFNameHomeCare);
            TextView txtLNameHomeCare = row.FindViewById<TextView>(Resource.Id.txtLNameHomeCare);
            TextView txtPackageLevelHomeCare = row.FindViewById<TextView>(Resource.Id.txtPackageLevelHomeCare);
            TextView txtPackageIncomeHomeCare = row.FindViewById<TextView>(Resource.Id.txtPackageIncomeHomeCare);

            switch (textSize)
            {
                case 0:
                    txtFNameHomeCare.TextSize = 10;
                    txtLNameHomeCare.TextSize = 10;
                    txtPackageLevelHomeCare.TextSize = 10;
                    txtPackageIncomeHomeCare.TextSize = 10;
                    break;
                case 1:
                    txtFNameHomeCare.TextSize = 15;
                    txtLNameHomeCare.TextSize = 15;
                    txtPackageLevelHomeCare.TextSize = 15;
                    txtPackageIncomeHomeCare.TextSize = 15;
                    break;
                case 2:
                    txtFNameHomeCare.TextSize = 20;
                    txtLNameHomeCare.TextSize = 20;
                    txtPackageLevelHomeCare.TextSize = 20;
                    txtPackageIncomeHomeCare.TextSize = 20;
                    break;
            }

            if (NightSwitchMode) {
                txtFNameHomeCare.SetTextColor(Color.White);
                txtLNameHomeCare.SetTextColor(Color.White);
                txtPackageLevelHomeCare.SetTextColor(Color.White);
                txtPackageIncomeHomeCare.SetTextColor(Color.White);
            } else {
                txtFNameHomeCare.SetTextColor(Color.Black);
                txtLNameHomeCare.SetTextColor(Color.Black);
                txtPackageLevelHomeCare.SetTextColor(Color.Black);
                txtPackageIncomeHomeCare.SetTextColor(Color.Black);
            }

            //for every item
            txtFNameHomeCare.Text = Items[position].GetResidentFirstName();
            txtLNameHomeCare.Text = Items[position].GetResidentLastName();
            txtPackageLevelHomeCare.Text = Items[position].GetPackageLevel().ToString();
            txtPackageIncomeHomeCare.Text = "$ " + Items[position].GetPackageIncome().ToString();
            //set indicator colours
            if (Items[position].IsGreen()){
                if (NightSwitchMode) {
                    row.SetBackgroundColor(Color.DarkGreen);
                } else {
                    row.SetBackgroundColor(Color.LightGreen);
                }
            } else if (Items[position].IsRed()) {
                if (NightSwitchMode) {
                    row.SetBackgroundColor(Color.DarkRed);
                } else {
                    row.SetBackgroundColor(Color.Argb(80, 255, 128, 128));
                }
            } else {
                if (NightSwitchMode) {
                    row.SetBackgroundColor(Color.DarkOrange);
                } else {
                    row.SetBackgroundColor(Color.LightYellow);
                }
            }
            
            return row;
        }

    }
}