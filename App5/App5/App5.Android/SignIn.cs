using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace MiCareApp.Droid
{
    class SignIn : DialogFragment
    {
        private WebClient client;
        private Uri url;
        private ISharedPreferences preferences;

        private EditText EmailTxt;
        private EditText PasswordTxt;
        private TextView SignInTxt;
        private Button SignInButton;
        private CheckBox RememberMe;

        private bool checkboxStatus;
        private string UserEmail;
        private string UserPassword;

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

            //Create shared preferences
            preferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            UserEmail = preferences.GetString("UserEmail", String.Empty);
            //UserPassword = preferences.GetString("UserPassword", String.Empty);
            checkboxStatus = preferences.GetBoolean("status", false);


            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/SignIn.php");

            //setup the edit text views identifed by the widget id values in SignIn.axml
            RememberMe = view.FindViewById<CheckBox>(Resource.Id.SignInBox);
            if (checkboxStatus) {
                RememberMe.Checked = true;
            }

            EmailTxt = view.FindViewById<EditText>(Resource.Id.SignInEmail);

            EmailTxt.Text = UserEmail;

            PasswordTxt = view.FindViewById<EditText>(Resource.Id.SignInPassW);

            PasswordTxt.Text = UserPassword;

            SignInTxt = view.FindViewById<TextView>(Resource.Id.SignInTxt);

            SignInButton = view.FindViewById<Button>(Resource.Id.SignInExecute);

            //when the button is clicked
            SignInButton.Click += delegate {

                SignInButton.Enabled = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Email", EmailTxt.Text);
                values.Add("Password", PasswordTxt.Text);

                client.UploadValuesCompleted += UploadValuesFinish;
                client.UploadValuesAsync(url, values);
            };

            return view;
        }

        private void UploadValuesFinish(object sender, UploadValuesCompletedEventArgs e)
        {
            string json = Encoding.UTF8.GetString(e.Result);
            List<User> tempUser = new List<User>();
            User currentUser = new User();
            tempUser = JsonConvert.DeserializeObject<List<User>>(json);

            foreach (User item in tempUser) {
                currentUser = item;
            }

            //if the emails match
            if (String.Equals(EmailTxt.Text, currentUser.GetEmail())) {
                //if the passwords match
                if (String.Equals(PasswordTxt.Text, currentUser.GetPassword())) {
                    if (RememberMe.Checked) {
                        ISharedPreferencesEditor edit = preferences.Edit();
                        edit.PutString("UserEmail", currentUser.GetEmail());
                        //edit.PutString("UserPassword", currentUser.GetPassword());//change later when hashing
                        edit.PutBoolean("status", true);
                        edit.Apply();//async version of commit 
                    } else {
                        ISharedPreferencesEditor edit = preferences.Edit();
                        edit.PutString("UserEmail", String.Empty);
                        //edit.PutString("UserPassword", String.Empty);
                        edit.PutBoolean("status", false);
                        edit.Apply();
                    };

                    //move from fragment to activity -> IntroPage.cs
                    Intent nextPage = new Intent(Activity, typeof(IntroPage));
                    nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));
                    StartActivity(nextPage);
                }
                else {
                    SignInTxt.Text = "Sorry, that password is incorrect";
                    SignInButton.Enabled = true;
                }
            } else {
                SignInTxt.Text = "Sorry, that email does not exist in our systems";
                SignInButton.Enabled = true;
            }

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