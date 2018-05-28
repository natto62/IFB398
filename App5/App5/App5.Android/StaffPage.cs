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
    [Activity(Label = "StaffPage", Theme = "@style/MainTheme")]
    public class StaffPage : Activity
    {

        private ListView dataList;
        private List<StaffData> dataItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.StaffPage);


            dataItems = new List<StaffData>();

            //setup Spinner
            Spinner spinner = FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            StaffViewAdapter adapter = new StaffViewAdapter(this, dataItems);

            dataList.Adapter = adapter;



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(OccupancyMenu)); };
        }
    }
}