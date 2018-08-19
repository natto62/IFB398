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
using Newtonsoft.Json;

namespace MiCareApp.Droid
{
    [Activity(Label = "IntroPage", Theme = "@style/MainTheme")]
    public class IntroPage : Activity
    {
        private User currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IntroPage);

            currentUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("UserData"));

            //ISharedPreferences preferences = Application.Context.GetSharedPreferences("UserInformation" + currentUser.GetUserID().ToString(), FileCreationMode.Private);
            //ISharedPreferencesEditor edit = preferences.Edit();
            //edit.PutInt("UserID", currentUser.GetUserID());
            //edit.PutString("UserEmail", currentUser.GetEmail());
            //edit.PutString("UserFName", currentUser.GetFirstName());
            //edit.PutString("UserLName", currentUser.GetLastName());
            //edit.Apply();

            TextView WelcomeTxt = FindViewById<TextView>(Resource.Id.WelcomeMessage);
            WelcomeTxt.Text = "Welcome " + currentUser.GetFirstName() + " " + currentUser.GetLastName() + " at " + currentUser.GetEmail();

            //Finance Menu button
            Button FinanceBtn = FindViewById<Button>(Resource.Id.FinanceBtn);
            //-> FinanceMenu.cs
            FinanceBtn.Click += delegate {
                Intent nextPage = new Intent(BaseContext, typeof(FinanceMenu));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));
                StartActivity(nextPage);
            };

            //Occupancy Menu button
            Button OccupancyBtn = FindViewById<Button>(Resource.Id.OccupancyBtn);
            //->OccupancyMenu.cs
            OccupancyBtn.Click += delegate {
                Intent nextPage = new Intent(BaseContext, typeof(OccupancyMenu));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));
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