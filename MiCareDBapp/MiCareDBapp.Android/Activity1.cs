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
using Newtonsoft.Json;
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "Activity1", Theme = "@style/MainTheme")]
    public class Activity1 : FragmentActivity
    {
        private string[] names;

        private User currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            currentUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("UserData"));

            //havn't commented this stuff yet, it will take a while
            names = new string[] { "ACFI Funding Data", "Agency Usage Data", "Bank Balance", "Brokerage Hours Data", "Home Care Package Data", "Income Data", "Salaries Wages Data" };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewPager);

            //set up the sliding layout bar adapter
            SlidingTabMenu adapter = new SlidingTabMenu(SupportFragmentManager, names, true);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = adapter;
            viewPager.OffscreenPageLimit = 6;

            //back button to return to finance menu page
            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);
            backBtn.Click += delegate {
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                Intent nextPage = new Intent(BaseContext, typeof(IntroPage));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));
                StartActivity(nextPage);
            };

            Button OptionsBtn = FindViewById<Button>(Resource.Id.OptionsButton);

            Settings SettingsScreen = new Settings(currentUser, OptionsBtn, adapter.GetFragments());

            OptionsBtn.Click += delegate {
                OptionsBtn.SetBackgroundResource(Resource.Drawable.OptionsIconClicked);
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                SettingsScreen.Show(transaction, "dialog fragment");
            };
        }
    }
}