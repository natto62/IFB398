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
using Newtonsoft.Json;
using Android;
using Android.Content.PM;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "IntroPage", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class IntroPage : Activity
    {
        private User currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreate(savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the IntroPage.axml file which is located in Resources/layout/
            SetContentView(Resource.Layout.IntroPage);

            //retrieve user object from SignIn.cs
            currentUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("UserData"));

            //store user id value in a shared preference for later use (to use when retrieving saved user settings in the kpi pages)
            ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            ISharedPreferencesEditor edit = preferences.Edit();
            edit.PutString("LatestUserID", currentUser.GetUserID().ToString());
            edit.Apply();

            //set the user's name and email in the welcome message
            TextView WelcomeTxt = FindViewById<TextView>(Resource.Id.WelcomeMessage);
            WelcomeTxt.Text = "Welcome " + currentUser.GetFirstName() + " " + currentUser.GetLastName() + " at " + currentUser.GetEmail();

            //Finance Menu button takes the user to the finance kpi page via view financial kpi's
            Button FinanceBtn = FindViewById<Button>(Resource.Id.FinanceBtn);
            FinanceBtn.Click += delegate {
                Intent nextPage = new Intent(BaseContext, typeof(SlidingTabFinancial));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));//use intent class to send local user object to the next activity file to be used
                StartActivity(nextPage);
            };

            //Occupancy Menu button the user to the occupancy kpi page via view occupancy kpi's
            Button OccupancyBtn = FindViewById<Button>(Resource.Id.OccupancyBtn);
            OccupancyBtn.Click += delegate {
                Intent nextPage = new Intent(BaseContext, typeof(SlidingTabOccupancy));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));//use intent class to send local user object to the next activity file to be used
                StartActivity(nextPage);
            };

            //Sign Out button
            Button SignOutBtn = FindViewById<Button>(Resource.Id.SignOutBtn);
            SignOutBtn.Click += delegate {

                StartActivity(typeof(MainActivity));
            };


        }
    }
}