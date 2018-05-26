using System;
using System.Collections.Generic;
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

            TextView txtDateSalariesWages = row.FindViewById<TextView>(Resource.Id.txtDateSalariesWages);
            txtDateSalariesWages.Text = Items[position].GetDate().ToShortDateString();

            TextView txtRosteredCostSalariesWages = row.FindViewById<TextView>(Resource.Id.txtRosteredCostSalariesWages);
            txtRosteredCostSalariesWages.Text = "$ " + Items[position].GetRosteredCost().ToString();

            TextView txtBudgetSalariesWages = row.FindViewById<TextView>(Resource.Id.txtBudgetSalariesWages);
            txtBudgetSalariesWages.Text = "$ " + Items[position].GetBudget().ToString();

            return row;
        }

    }
}