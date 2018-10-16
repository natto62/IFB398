using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "SlidingTabOccupancy", Theme = "@style/MainTheme")]
    public class SlidingTabOccupancy : FragmentActivity
    {
        private string[] names;

        private User currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            currentUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("UserData"));

            //havn't commented this stuff yet, it will take a while
            names = new string[] { "Occupancy Data", "Staff Attendance Data"};

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewPagerOccupancy);

            //set up the sliding layout bar adapter
            SlidingTabMenu adapter = new SlidingTabMenu(SupportFragmentManager, names, false);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = adapter;

            //back button to return to finance menu page
            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);
            backBtn.Click += delegate {
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                Intent nextPage = new Intent(BaseContext, typeof(IntroPage));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));
                StartActivity(nextPage);
            };

            Button OptionsBtn = FindViewById<Button>(Resource.Id.OptionsButton);

            Settings SettingsScreen = new Settings(currentUser, OptionsBtn);

            OptionsBtn.Click += delegate {
                OptionsBtn.SetBackgroundResource(Resource.Drawable.OptionsIconClicked);
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                SettingsScreen.Show(transaction, "dialog fragment");
            };
        }
    }
}