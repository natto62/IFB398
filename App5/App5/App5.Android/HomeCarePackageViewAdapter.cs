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
using App5.Droid;

namespace MiCareApp.Droid 
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

            TextView txtFNameHomeCare = row.FindViewById<TextView>(Resource.Id.txtFNameHomeCare);
            txtFNameHomeCare.Text = Items[position].GetResidentFirstName();

            TextView txtLNameHomeCare = row.FindViewById<TextView>(Resource.Id.txtLNameHomeCare);
            txtLNameHomeCare.Text = Items[position].GetResidentLastName();

            TextView txtPackageLevelHomeCare = row.FindViewById<TextView>(Resource.Id.txtPackageLevelHomeCare);
            txtPackageLevelHomeCare.Text = Items[position].GetPackageLevel().ToString();

            TextView txtPackageIncomeHomeCare = row.FindViewById<TextView>(Resource.Id.txtPackageIncomeHomeCare);
            txtPackageIncomeHomeCare.Text = "$ " + Items[position].GetPackageIncome().ToString();

            if (Items[position].IsGreen()){
                row.SetBackgroundColor(Color.LightGreen);
            } else if (Items[position].IsRed()) {
                row.SetBackgroundColor(Color.Argb(80,255,128,128));
            } else {
                row.SetBackgroundColor(Color.LightYellow);
            }
            
            return row;
        }

    }
}