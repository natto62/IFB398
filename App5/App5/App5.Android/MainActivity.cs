﻿using System;

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
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreate(bundle);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the Home.axml file which is located in Resources/layout/
            SetContentView(Resource.Layout.Home);

            //identifies which widgets are the sign in and sign out buttons by using id identifiers found in the axml file
            Button signIn = FindViewById<Button>(Resource.Id.SignInBtn);

            signIn.Click += SignIn_ItemClick;//calls a method once clicked

            Button signUp = FindViewById<Button>(Resource.Id.SignUpBtn);

            signUp.Click += SignUp_ItemClick;//calls a method once clicked

        }

        //create a pop up window with more information
        void SignIn_ItemClick(object sender, EventArgs e) {
            //fragment manager manages fragments in android, handles transactions between fragments, 
            //a transaction is a way to add, replace or remove fragments
            //more info on fragments: https:/developer.android.com/guide/components/fragments
            //it represents a portion of the user interface such as a pop up window
            FragmentTransaction transaction = FragmentManager.BeginTransaction();

            //this user object only used for testing
            DateTime TempTime = new DateTime(2018, 1, 1, 9, 20, 00);
            User tempUser = new User("Brian", "Who", "super1@blabla.com", "qwerty1", TempTime);

            //Go to SignIn.cs for more
            SignIn SignInPopUp = new SignIn(tempUser);
            //show the dialog fragment which will be a pop up window
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

