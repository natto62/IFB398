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


        }
    }
}