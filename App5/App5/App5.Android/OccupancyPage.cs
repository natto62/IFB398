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
    [Activity(Label = "OccupancyPage", Theme = "@style/MainTheme")]
    public class OccupancyPage : Activity
    {

        private ListView dataList;
        private List<OccupancyData> dataItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.OccupancyPage);


            dataItems = new List<OccupancyData>();

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            OccupancyViewAdapter adapter = new OccupancyViewAdapter(this, dataItems);

            dataList.Adapter = adapter;



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(OccupancyMenu)); };
        }
    }
}