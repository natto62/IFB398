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

            signIn.Click += delegate { StartActivity(typeof(ItemPage)); };

        }

    }
}

