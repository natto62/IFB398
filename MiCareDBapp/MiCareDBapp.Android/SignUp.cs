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
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.SignUp, container, false);

            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/SignUp.php");

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
                    // User newUser = new User();
                    SignUpTxt.Text = "Creating User please wait...";
                    //add post values to send to the php file
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
            string json = Encoding.UTF8.GetString(e.Result);
            SignUpBtn.Enabled = true;
            SignUpTxt.Text = json;
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {

            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }
    }
}