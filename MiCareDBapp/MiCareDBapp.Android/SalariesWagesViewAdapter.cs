﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
    class SalariesWagesViewAdapter : BaseAdapter<SalariesWagesData>
    {

        private List<SalariesWagesData> Items;
        private Context Context;

        public SalariesWagesViewAdapter(Context context, List<SalariesWagesData> items)
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

        public override SalariesWagesData this[int position]
        {
            get { return Items[position]; }
        }

        //set the view
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(Context).Inflate(Resource.Layout.SalariesWagesTable, null, false);
            }
            //retrieve the shared preferences to edit the row attributes such as text size, colour or if the the data has to be sorted by date
            ISharedPreferences getidpreferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            string UserID = getidpreferences.GetString("LatestUserID", String.Empty);
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + UserID, FileCreationMode.Private);
            int textSize = preferences.GetInt("TextSize", 1);
            bool NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            bool DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);

            TextView txtDateSalariesWages = row.FindViewById<TextView>(Resource.Id.txtDateSalariesWages);
            TextView txtRosteredCostSalariesWages = row.FindViewById<TextView>(Resource.Id.txtActualCostSalariesWages);
            TextView txtBudgetSalariesWages = row.FindViewById<TextView>(Resource.Id.txtBudgetCostSalariesWages);
            TextView txtVarianceSalariesWages = row.FindViewById<TextView>(Resource.Id.txtVarianceSalariesWages);

            switch (textSize)
            {
                case 0:
                    txtDateSalariesWages.TextSize = 10;
                    txtRosteredCostSalariesWages.TextSize = 10;
                    txtBudgetSalariesWages.TextSize = 10;
                    txtVarianceSalariesWages.TextSize = 10;
                    break;
                case 1:
                    txtDateSalariesWages.TextSize = 15;
                    txtRosteredCostSalariesWages.TextSize = 15;
                    txtBudgetSalariesWages.TextSize = 15;
                    txtVarianceSalariesWages.TextSize = 15;
                    break;
                case 2:
                    txtDateSalariesWages.TextSize = 20;
                    txtRosteredCostSalariesWages.TextSize = 20;
                    txtBudgetSalariesWages.TextSize = 20;
                    txtVarianceSalariesWages.TextSize = 20;
                    break;
            }

            if (NightSwitchMode) {
                txtDateSalariesWages.SetTextColor(Color.White);
                txtRosteredCostSalariesWages.SetTextColor(Color.White);
                txtBudgetSalariesWages.SetTextColor(Color.White);
                txtVarianceSalariesWages.SetTextColor(Color.White);
            } else {
                txtDateSalariesWages.SetTextColor(Color.Black);
                txtRosteredCostSalariesWages.SetTextColor(Color.Black);
                txtBudgetSalariesWages.SetTextColor(Color.Black);
                txtVarianceSalariesWages.SetTextColor(Color.Black);
            }

            if (DateSwitchMode) {
                Items.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                    return DateTime.Compare(one.GetDate(), two.GetDate());
                });
            }

            //for every item
            txtDateSalariesWages.Text = Items[position].GetDate().ToShortDateString();
            txtRosteredCostSalariesWages.Text = "$ " + Items[position].GetActualCost().ToString("#,#", CultureInfo.InvariantCulture);
            txtVarianceSalariesWages.Text = "$ " + Items[position].GetVariance().ToString("#,#", CultureInfo.InvariantCulture);
            txtBudgetSalariesWages.Text = "$ " + Items[position].GetBudget().ToString("#,#", CultureInfo.InvariantCulture);
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