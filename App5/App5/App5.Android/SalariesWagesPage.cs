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
    [Activity(Label = "SalariesWagesPage", Theme = "@style/MainTheme")]
    public class SalariesWagesPage : Activity
    {

        private ListView dataList;
        private List<SalariesWagesData> dataItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SalariesWagesPage);


            dataItems = new List<SalariesWagesData>();

            //setup Spinner
            Spinner spinner = FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            SalariesWagesViewAdapter adapter = new SalariesWagesViewAdapter(this, dataItems);

            dataList.Adapter = adapter;



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };
        }
    }
}