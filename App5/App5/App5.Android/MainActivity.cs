using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using App5.Droid;
using System.Collections.Generic;

namespace MiCareApp.Droid
{
    [Activity(Label = "App5", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Home);
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());


            Button signIn = FindViewById<Button>(Resource.Id.SignInBtn);

            signIn.Click += SignIn_ItemClick;

            Button signUp = FindViewById<Button>(Resource.Id.SignUpBtn);

            signUp.Click += SignUp_ItemClick;

        }

        //create a pop up window with more information
        void SignIn_ItemClick(object sender, EventArgs e) {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();

            //this user object only used for testing
            DateTime TempTime = new DateTime(2018, 1, 1, 9, 20, 00);
            User tempUser = new User("Brian", "Who", "super1@blabla.com", "qwerty1", TempTime);

            SignIn SignInPopUp = new SignIn(tempUser);
            SignInPopUp.Show(transaction, "dialog fragment");
        }

        //create a pop up window with more information
        void SignUp_ItemClick(object sender, EventArgs e) {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SignUp SignUpPopUp = new SignUp();
            SignUpPopUp.Show(transaction, "dialog fragment");
        }

    }
}

