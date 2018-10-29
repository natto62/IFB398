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
using Android.Content.PM;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "SlidingTabFinancial", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SlidingTabFinancial : FragmentActivity
    {
        private string[] names;

        private User currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //retrieve user object from IntroPage.cs
            currentUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("UserData"));

            //list of strings to use for the sliding tab interface, these are the titles of the different financial kpi's each with their own fragment
            names = new string[] { "ACFI Funding Data", "Agency Usage Data", "Bank Balance", "Brokerage Hours Data", "Home Care Package Data", "Income Data", "Salaries Wages Data" };

            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreate(savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the ViewPager.axml file which is located in Resources/layout/
            SetContentView(Resource.Layout.ViewPager);

            //set up the sliding layout bar adapter from the class created in SlidingTabMenu.cs with string names assigned above
            SlidingTabMenu adapter = new SlidingTabMenu(SupportFragmentManager, names, true);

            //set the view pager and assign its adapter the sliding tab menu custom adapter
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = adapter;
            //set the off screen page limit to 6 (the default = 1) as there are 7 kpi's, so 7 tabs, (say the user pulled data from the last tab and then went to the first tab,
            //the data from the last tab would go away and the user would have to repull the that data again if they go back to the last tab, this if if the page limit was 1), with no page limit
            //the data from other pages will not get erased if the user swaps to far away tabs
            viewPager.OffscreenPageLimit = 6;

            //back button to return to IntroPage.cs
            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);
            backBtn.Click += delegate {
                //set the back btn image to a lighter shade of green
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                Intent nextPage = new Intent(BaseContext, typeof(IntroPage));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));//use intent class to send local user object to the next activity file to be used
                StartActivity(nextPage);
            };
            
            Button OptionsBtn = FindViewById<Button>(Resource.Id.OptionsButton);

            //set up the settings pop up window, go to Settings.cs for more, input all fragments from the sliding tab menu adapter, this is so all fragments can get updated when
            //settings are changed
            //current user object is used to check for saved shared preference settings for the user ID associated with this user
            Settings SettingsScreen = new Settings(currentUser, OptionsBtn, adapter.GetFragments());

            //open settings pop up window when options button is touched
            OptionsBtn.Click += delegate {
                //set the option btn image to a lighter shade of green
                OptionsBtn.SetBackgroundResource(Resource.Drawable.OptionsIconClicked);
                //fragment manager manages fragments in android, handles transactions between fragments, 
                //a transaction is a way to add, replace or remove fragments
                //more info on fragments: https:/developer.android.com/guide/components/fragments
                //it represents a portion of the user interface such as a pop up window
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                //show the dialog fragment which will be a pop up window
                SettingsScreen.Show(transaction, "dialog fragment");
            };
        }
    }
}