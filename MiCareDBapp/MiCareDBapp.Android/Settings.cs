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
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Settings, container, false);

            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.TextSizeSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.TextArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;
            spinner.SetSelection(textSize);

            Switch NightSwitch = view.FindViewById<Switch>(Resource.Id.NightSwitch);
            if (NightSwitchMode) {
                NightSwitch.Checked = true;
            }
            
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

            Switch DateSwitch = view.FindViewById<Switch>(Resource.Id.DateSwitch);
            if (DateSwitchMode) {
                DateSwitch.Checked = true;
            }

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

            client = new WebClient();
            urlUser = new Uri("https://capstonephpcode198.herokuapp.com/ChangeUserName.php");
            urlPass = new Uri("https://capstonephpcode198.herokuapp.com/ChangePassword.php");

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

                    client.UploadValuesCompleted += UploadValuesFinishUser;
                    client.UploadValuesAsync(urlUser, values);
                }

            };

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
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }

        public override void OnCancel(IDialogInterface dialog) {
            base.OnCancel(dialog);
            OptionsBtn.SetBackgroundResource(Resource.Drawable.OptionsIcon);
        }

        public void updateInterfaceNow() { 
            foreach (Android.Support.V4.App.Fragment fragment in fragmentItems) {
                object item = fragment;
                MethodInfo method = item.GetType().GetMethod("NotifyAdapter");
                method.Invoke(item, null);
            }    
        }
    }
}