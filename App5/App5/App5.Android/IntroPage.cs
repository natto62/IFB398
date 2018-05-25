using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using App5.Droid;
using Android.Widget;

namespace MiCareApp.Droid
{
    [Activity(Label = "IntroPage", Theme = "@style/MainTheme")]
    public class IntroPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IntroPage);

            //Finance Menu
            Button FinanceBtn = FindViewById<Button>(Resource.Id.FinanceBtn);
            FinanceBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };

            //Occupancy Menu
            Button OccupancyBtn = FindViewById<Button>(Resource.Id.OccupancyBtn);
            OccupancyBtn.Click += delegate { StartActivity(typeof(OccupancyMenu)); };

            //Sign Out
            Button SignOutBtn = FindViewById<Button>(Resource.Id.SignOutBtn);
            SignOutBtn.Click += delegate { StartActivity(typeof(MainActivity)); };


        }
    }
}