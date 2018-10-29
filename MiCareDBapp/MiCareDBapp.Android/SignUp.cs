using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace MiCareDBapp.Droid
{
    class SignUp : DialogFragment
    {
        private WebClient client;
        private Uri url;

        private Button SignUpBtn;
        private TextView SignUpTxt;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Called to have the fragment instantiate its user interface view
            //LayoutInflater: The LayoutInflater object that can be used to inflate any views in the fragment
            //ViewGroup: If non-null, this is the parent view that the fragment's UI should be attached to. The fragment should not add the view itself, but this can be used to generate the LayoutParams of the view.
            //Bundle: If non-null, this fragment is being re-constructed from a previous saved state as given here.
            //reference: https:/developer.android.com/reference/android/app/Fragment.html#onCreateView(android.view.LayoutInflater,%20android.view.ViewGroup,%20android.os.Bundle)
            base.OnCreateView(inflater, container, savedInstanceState);

            //the view being inflated is SignUp.axml from Resources/layout/
            var view = inflater.Inflate(Resource.Layout.SignUp, container, false);

            //create new WebClient class object which can provide methods to push and recieve data from an online resource via a url
            client = new WebClient();
            //set the url to push and pull data from, via the a Uri class object
            //the online resource is a php file hosted on heroku, these php files read write and pull database tables
            url = new Uri("https://capstonephpcode198.herokuapp.com/SignUp.php");

            //A list for the text fields
            List<EditText> EditTextList = new List<EditText>(); 

            //edit text widgets where users can input data
            EditText FNameTxt = view.FindViewById<EditText>(Resource.Id.SignUpFirstName);
            EditTextList.Add(FNameTxt);
            EditText LNameTxt = view.FindViewById<EditText>(Resource.Id.SignUpLastName);
            EditTextList.Add(LNameTxt);
            EditText EmailTxt = view.FindViewById<EditText>(Resource.Id.SignUpEmail);
            EditTextList.Add(EmailTxt);
            EditText PassWTxt = view.FindViewById<EditText>(Resource.Id.SignUpPassW);
            EditTextList.Add(PassWTxt);
            EditText CPassWTxt = view.FindViewById<EditText>(Resource.Id.SignUpCPassW);
            EditTextList.Add(CPassWTxt);

            //sign up text field
            SignUpTxt = view.FindViewById<TextView>(Resource.Id.SignUpTxt);

            //sign up confirmation button
            SignUpBtn = view.FindViewById<Button>(Resource.Id.SignUpExecute);

            //boolean value specifying if the user can sign up or not
            bool signUpYes = false;

            SignUpBtn.Click += delegate {
                //disable the button untill action is complete
                SignUpBtn.Enabled = false;

                //if any text field is unfilled
                foreach (EditText item in EditTextList) {
                    if (item.Text.Length == 0) {
                        signUpYes = false;
                        item.SetHintTextColor(Color.DarkRed);
                        SignUpTxt.Text = "All fields need to be filled";
                        SignUpBtn.Enabled = true;
                    } else {
                        signUpYes = true;
                    }
                }

                //if the password text field and the confirm password text field do not match
                if (PassWTxt.Text != CPassWTxt.Text) {
                    signUpYes = false;
                    CPassWTxt.SetHintTextColor(Color.DarkRed);
                    SignUpTxt.Text = "Please reconfirm Password";
                    SignUpBtn.Enabled = true;
                }
                if (signUpYes) {
                    SignUpTxt.Text = "Creating User please wait...";

                    //add post values to send to the php file to input into database
                    NameValueCollection values = new NameValueCollection();
                    values.Add("FName", FNameTxt.Text);
                    values.Add("LName", LNameTxt.Text);
                    values.Add("Email", EmailTxt.Text);
                    values.Add("Password", PassWTxt.Text);

                    client.UploadValuesCompleted += UploadValuesFinish;
                    client.UploadValuesAsync(url, values);
                }
            };
            return view;
        }

        private void UploadValuesFinish(object sender, UploadValuesCompletedEventArgs e) {
            //outputs message to user if the user has been entered into the database or if the email already exists, messages are in SignUp.php
            string json = Encoding.UTF8.GetString(e.Result);
            SignUpBtn.Enabled = true;
            SignUpTxt.Text = json;
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