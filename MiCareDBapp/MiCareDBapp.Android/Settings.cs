using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace MiCareDBapp.Droid
{
    public class Settings : DialogFragment
    {
        private WebClient client;
        private Uri urlUser;
        private Uri urlPass;

        private User currentUser;
        private Button OptionsBtn;
        private List<Android.Support.V4.App.Fragment> fragmentItems;
        private ISharedPreferences preferences;
        private ISharedPreferencesEditor edit;

        private int textSize;
        private bool NightSwitchMode;
        private bool DateSwitchMode;

        private TextView NameResult;
        private TextView PassResult;
        private Button NameResetBtn;
        private Button PasswordResetBtn;
        private EditText FNameReset;
        private EditText LNameReset;

        //constructor assigns the current user, options btn, all kpi fragments on the page, and retrieves shared preferences based on the current user
        public Settings(User data, Button button, List<Android.Support.V4.App.Fragment> fragments) {
            currentUser = data;
            OptionsBtn = button;
            fragmentItems = fragments;

            preferences = Application.Context.GetSharedPreferences("UserInformation" + currentUser.GetUserID().ToString(), FileCreationMode.Private);
            textSize = preferences.GetInt("TextSize", 1);
            NightSwitchMode = preferences.GetBoolean("NightSwitchMode", false);
            DateSwitchMode = preferences.GetBoolean("DateSwitchMode", false);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the Settings.axml file which is located in Resources/layout/
            var view = inflater.Inflate(Resource.Layout.Settings, container, false);

            //setup spinner for text size
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.TextSizeSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.TextArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;
            spinner.SetSelection(textSize);//set the default selection based on shared preferences for this user

            //setup night switch which changes the colour of the data rows to a darker hue
            Switch NightSwitch = view.FindViewById<Switch>(Resource.Id.NightSwitch);
            if (NightSwitchMode)
            {//set the default selection based on shared preferences for this user
                NightSwitch.Checked = true;
            }
            //change rows to a darker or lighter hue, and edit the shared preferences for this user
            NightSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                edit = preferences.Edit();
                if (e.IsChecked) {
                    edit.PutBoolean("NightSwitchMode", true);
                } else {
                    edit.PutBoolean("NightSwitchMode", false);
                }
                edit.Apply();
                updateInterfaceNow();
            };
            //setup date switch, which sorts the data via date default when being pulled from database
            Switch DateSwitch = view.FindViewById<Switch>(Resource.Id.DateSwitch);
            if (DateSwitchMode)
            {//set the default selection based on shared preferences for this user
                DateSwitch.Checked = true;
            }
            //sorts the data via date default when being pulled from database, and edit the shared preferences for this user
            DateSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                edit = preferences.Edit();
                if (e.IsChecked) {
                    edit.PutBoolean("DateSwitchMode", true);
                } else {
                    edit.PutBoolean("DateSwitchMode", false);
                }
                edit.Apply();
                updateInterfaceNow();
            };
            //create new WebClient class object which can provide methods to push and recieve data from an online resource via a url
            client = new WebClient();
            //set the url to push and pull data from, via the a Uri class object
            //the online resource is a php file hosted on heroku, these php files read write and pull database tables
            urlUser = new Uri("https://capstonephpcode198.herokuapp.com/ChangeUserName.php");
            urlPass = new Uri("https://capstonephpcode198.herokuapp.com/ChangePassword.php");

            //setup widgets to allow the user to change their name
            FNameReset = view.FindViewById<EditText>(Resource.Id.FirstNameReset);

            LNameReset = view.FindViewById<EditText>(Resource.Id.LastNameReset);

            NameResult = view.FindViewById<TextView>(Resource.Id.NameChangeResult);

            NameResetBtn = view.FindViewById<Button>(Resource.Id.NameChangeBtn);

            NameResetBtn.Click += delegate {
                if (FNameReset.Text.Length == 0 || LNameReset.Text.Length == 0) {
                    NameResult.Text = "Cannot have a blank first and/or last name.";
                } else {
                    NameResetBtn.Enabled = false;
                    NameResult.Text = "Changing name please wait...";
                    NameValueCollection values = new NameValueCollection();
                    values.Add("Email", currentUser.GetEmail());
                    values.Add("FName", FNameReset.Text);
                    values.Add("LName", LNameReset.Text);
                    //call php file and use UploadValuesAsync with the user input values to send to the database
                    client.UploadValuesCompleted += UploadValuesFinishUser;
                    client.UploadValuesAsync(urlUser, values);
                }

            };
            //setup widgets to allow the user to change their password
            EditText OldPassWReset = view.FindViewById<EditText>(Resource.Id.OldPasswordReset);

            EditText NewPassWReset = view.FindViewById<EditText>(Resource.Id.PasswordReset);

            EditText CNewPassWReset = view.FindViewById<EditText>(Resource.Id.ConfirmPasswordReset);

            PassResult = view.FindViewById<TextView>(Resource.Id.PasswordChangeResult);

            PasswordResetBtn = view.FindViewById<Button>(Resource.Id.PasswordChangeBtn);

            PasswordResetBtn.Click += delegate {
                if (NewPassWReset.Text.Length == 0 || CNewPassWReset.Text.Length == 0) {
                    PassResult.Text = "Cannot have a blank password.";
                } else {
                    PasswordResetBtn.Enabled = false;
                    PassResult.Text = "Changing password please wait...";
                    NameValueCollection values = new NameValueCollection();
                    values.Add("Email", currentUser.GetEmail());
                    values.Add("OPassword", OldPassWReset.Text);
                    values.Add("NPassword", NewPassWReset.Text);
                    values.Add("NPasswordC", CNewPassWReset.Text);
                    //call php file and use UploadValuesAsync with the user input values to send to the database
                    client.UploadValuesCompleted += UploadValuesFinishPass;
                    client.UploadValuesAsync(urlPass, values);
                }
            };

            return view;
        }

        private void UploadValuesFinishUser(object sender, UploadValuesCompletedEventArgs e) {
            string json = Encoding.UTF8.GetString(e.Result);
            NameResult.Text = json;
            NameResetBtn.Enabled = true;
            if (String.Equals(json, "Name has been changed.")) {
                currentUser.FName = FNameReset.Text;
                currentUser.LName = LNameReset.Text;
            }
        }

        private void UploadValuesFinishPass(object sender, UploadValuesCompletedEventArgs e) {
            string json = Encoding.UTF8.GetString(e.Result);
            PassResult.Text = json;
            PasswordResetBtn.Enabled = true;
        }
        //spinner to edit the text size, three options available
        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            edit = preferences.Edit();
            if (e.Position == 0) {
                edit.PutInt("TextSize", 0);

            } else if (e.Position == 1) {
                edit.PutInt("TextSize", 1);

            } else if (e.Position == 2) {
                edit.PutInt("TextSize", 2);

            }
            edit.Apply();
            updateInterfaceNow();
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            //setup animations and color properties
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //animation can be found in styles.xml in Resources/values/
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }

        public override void OnCancel(IDialogInterface dialog) {
            base.OnCancel(dialog);
            OptionsBtn.SetBackgroundResource(Resource.Drawable.OptionsIcon);//set the options icon to default upon closing
        }

        //updates the custom list view adapters of all the kpi's on the page using the methodInfo class
        public void updateInterfaceNow() { 
            foreach (Android.Support.V4.App.Fragment fragment in fragmentItems) {
                object item = fragment;
                MethodInfo method = item.GetType().GetMethod("NotifyAdapter");
                method.Invoke(item, null);
            }    
        }
    }
}