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
    [Activity(Label = "AgencyUsagePage", Theme = "@style/MainTheme")]
    public class AgencyUsagePage : Activity
    {

        private ListView dataList;
        private List<AgencyUsageData> dataItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AgencyUsagePage);


            dataItems = new List<AgencyUsageData>();

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            AgencyUsageViewAdapter adapter = new AgencyUsageViewAdapter(this, dataItems);

            dataList.Adapter = adapter;



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };
        }
    }
}