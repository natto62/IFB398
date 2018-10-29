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

            //Create shared preferences which is an interface to access and store prefernce data which will be stored and can be retrieved even after restarting the app
            preferences = Application.Context.GetSharedPreferences("UserInformation", FileCreationMode.Private);
            UserEmail = preferences.GetString("UserEmail", String.Empty);
            //UserPassword = preferences.GetString("UserPassword", String.Empty); was originally going to store the user's password, once the rememberMe checkbox was checked, however this idea was scrapped for now, for security 
            checkboxStatus = preferences.GetBoolean("status", false);

            //create new WebClient class object which can provide methods to push and recieve data from an online resource via a url
            client = new WebClient();
            //set the url to push and pull data from, via the a Uri class object
            //the online resource is a php file hosted on heroku, these php files read write and pull database tables
            url = new Uri("https://capstonephpcode198.herokuapp.com/SignIn.php");

            //Remember me check box will be checked if shared preferences variable "checkboxstatus" returns true
            RememberMe = view.FindViewById<CheckBox>(Resource.Id.SignInBox);
            if (checkboxStatus) {
                RememberMe.Checked = true;
            }

            //setup the edit text views identifed by the widget id values in SignIn.axml
            EmailTxt = view.FindViewById<EditText>(Resource.Id.SignInEmail);

            EmailTxt.Text = UserEmail;//set text to the stored users email

            PasswordTxt = view.FindViewById<EditText>(Resource.Id.SignInPassW);

            SignInTxt = view.FindViewById<TextView>(Resource.Id.SignInTxt);//displays output messages

            SignInButton = view.FindViewById<Button>(Resource.Id.SignInExecute);

            //when the sign in button is clicked
            SignInButton.Click += delegate {
                SignInTxt.Text = "Signing in, please wait...";
                SignInButton.Enabled = false;

                //send post values to the php file, email and password, to check with database if it matches
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
            //get output string from php file
            string json = Encoding.UTF8.GetString(e.Result);
            //if the sign in credentials do not exist in the database
            if (String.Equals(json,"Sorry, that email does not exist in our systems.") || String.Equals(json, "Sorry, the given password is incorrect.")) {
                SignInTxt.Text = json;
                SignInButton.Enabled = true;
            //if the sign in credentials exist in the database
            } else {
                List <User> tempUser = new List<User>();
                User currentUser = new User();
                tempUser = JsonConvert.DeserializeObject<List<User>>(json);
                //use json to take user object from database and assign it to a local user class object
                foreach (User item in tempUser) {
                    currentUser = item;
                }
                //if the remember me check box is checked put the email in the shared preferences interface
                if (RememberMe.Checked) {
                    ISharedPreferencesEditor edit = preferences.Edit();//edit the shared preferences
                    edit.PutString("UserEmail", currentUser.GetEmail());
                    edit.PutBoolean("status", true);
                    edit.Apply();//async version of commit

                //if the remember me check box is not checked take out the email in the shared preferences interface if it exists, and turn off check box in shared preferences
                } else {
                    ISharedPreferencesEditor edit = preferences.Edit();//edit the shared preferences
                    edit.PutString("UserEmail", String.Empty);
                    edit.PutBoolean("status", false);
                    edit.Apply();
                };

                //move from fragment to activity -> IntroPage.cs
                Intent nextPage = new Intent(Activity, typeof(IntroPage));
                nextPage.PutExtra("UserData", JsonConvert.SerializeObject(currentUser));//use intent class to send local user object to the next activity file to be used
                StartActivity(nextPage);

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