﻿using System;
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

namespace MiCareApp.Droid
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
            names = new string[] { "Agency Usage Data", "Brokerage Hours Data", "Home Care Package Data", "Salaries Wages Data" };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewPager);

            //set up the sliding layout bar adapter
            SlidingTabMenu adapter = new SlidingTabMenu(SupportFragmentManager, names, true);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = adapter;

            //viewPager. += delegate
            //{

           // }

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