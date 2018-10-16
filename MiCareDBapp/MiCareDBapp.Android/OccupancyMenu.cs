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
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "OccupancyMenu", Theme = "@style/MainTheme")]
    public class OccupancyMenu : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.OccupancyDataList);

            //Back button
            Button BackBtn = FindViewById<Button>(Resource.Id.OccupancyBackButton);
            BackBtn.Click += delegate { StartActivity(typeof(IntroPage)); };

            //OccupancyBtn2
            Button OccupancyBtn2 = FindViewById<Button>(Resource.Id.OccupancyBtn2);
            OccupancyBtn2.Click += delegate { StartActivity(typeof(OccupancyPage)); };

            //StaffBtn
            Button StaffBtn = FindViewById<Button>(Resource.Id.StaffBtn);
            StaffBtn.Click += delegate { StartActivity(typeof(StaffPage)); };

        }
    }
}