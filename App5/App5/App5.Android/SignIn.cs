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
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.SignIn, container, false);

            EditText EmailTxt = view.FindViewById<EditText>(Resource.Id.SignInEmail);

            EditText PasswordTxt = view.FindViewById<EditText>(Resource.Id.SignInPassW);

            TextView SignInTxt = view.FindViewById<TextView>(Resource.Id.SignInTxt);

            Button SignInButton = view.FindViewById<Button>(Resource.Id.SignInExecute);

            SignInButton.Click += delegate {

                if (String.Equals(EmailTxt.Text, UserObject.GetEmail())) {
                    if (String.Equals(PasswordTxt.Text, UserObject.GetPassword())) {
                        //move from fragment to activity
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
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }
    }
}