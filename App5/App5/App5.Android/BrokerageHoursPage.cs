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
    [Activity(Label = "BrokerageHoursPage", Theme = "@style/MainTheme")]
    public class BrokerageHoursPage : Activity
    {

        private ListView dataList;
        private List<BrokerageHoursData> dataItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BrokerageHoursPage);


            dataItems = new List<BrokerageHoursData>();

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            BrokerageHoursViewAdapter adapter = new BrokerageHoursViewAdapter(this, dataItems);

            dataList.Adapter = adapter;



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };
        }
    }
}