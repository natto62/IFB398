using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App5.Droid;

namespace MiCareApp.Droid
{
    class SignIn : DialogFragment
    {
        private User UserObject;

        //only used for testing
        public SignIn(User data) {
            UserObject = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Called to have the fragment instantiate its user interface view
            //LayoutInflater: The LayoutInflater object that can be used to inflate any views in the fragment
            //ViewGroup: If non-null, this is the parent view that the fragment's UI should be attached to. The fragment should not add the view itself, but this can be used to generate the LayoutParams of the view.
            //Bundle: If non-null, this fragment is being re-constructed from a previous saved state as given here.
            //reference: https:/developer.android.com/reference/android/app/Fragment.html#onCreateView(android.view.LayoutInflater,%20android.view.ViewGroup,%20android.os.Bundle)
            base.OnCreateView(inflater, container, savedInstanceState);

            //the view being inflated is SignIn.axml from Resources/layout/
            var view = inflater.Inflate(Resource.Layout.SignIn, container, false);

            //setup the edit text views identifed by the widget id values in SignIn.axml
            EditText EmailTxt = view.FindViewById<EditText>(Resource.Id.SignInEmail);

            EditText PasswordTxt = view.FindViewById<EditText>(Resource.Id.SignInPassW);

            TextView SignInTxt = view.FindViewById<TextView>(Resource.Id.SignInTxt);

            Button SignInButton = view.FindViewById<Button>(Resource.Id.SignInExecute);

            //when the button is clicked
            SignInButton.Click += delegate {
                //if the emails match
                if (String.Equals(EmailTxt.Text, UserObject.GetEmail())) {
                    //if the passwords match
                    if (String.Equals(PasswordTxt.Text, UserObject.GetPassword())) {
                        //move from fragment to activity -> IntroPage.cs
                        StartActivity(new Intent(Activity, typeof(IntroPage)));
                    } else {
                        SignInTxt.Text = "Sorry, that password is incorrect";
                    }
                } else {
                    SignInTxt.Text = "Sorry, that email does not exist in our systems";
                }
                
            };

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {

            base.OnActivityCreated(savedInstanceState);
            //setup animations and color properties
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //animation can be found in styles.xml in Resources/values/
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }
    }
}