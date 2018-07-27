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
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace MiCareApp.Droid
{
    [Activity(Label = "Activity1")]
    public class Activity1 : FragmentActivity
    {
        private string[] names;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            names = new string[] { "Agency Usage Data", "Brokerage Hours Data", "Home Care Package Data", "Salaries Wages Data" };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewPager);

            SlidingTabMenu adapter = new SlidingTabMenu(SupportFragmentManager, names);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = adapter;
            
        }
    }
}