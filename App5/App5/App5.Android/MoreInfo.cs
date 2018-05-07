using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MiCareApp.Droid;

namespace App5.Droid
{ 
    class MoreInfo : DialogFragment
    {
        private FinanceData dataObject;

        private TextView FName;
        private TextView LName;
        private TextView Age;
        private TextView Gender;
        private TextView Income;
        private TextView Status;

        //import the finance data item based on the item clicked 
        public MoreInfo(FinanceData data) {
            dataObject = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DataPage2, container, false);


            //change the text based on the item clicked on the list
            FName = view.FindViewById<TextView>(Resource.Id.txtFName);
            FName.Text = dataObject.GetFirstName();

            LName = view.FindViewById<TextView>(Resource.Id.txtLName);
            LName.Text = dataObject.GetLastName();

            Age = view.FindViewById<TextView>(Resource.Id.txtAge);
            Age.Text = dataObject.GetAge();

            Gender = view.FindViewById<TextView>(Resource.Id.txtGender);
            Gender.Text = dataObject.GetGender();

            Income = view.FindViewById<TextView>(Resource.Id.txtIncome);
            Income.Text = "$ " + dataObject.GetIncome();

            Status = view.FindViewById<TextView>(Resource.Id.txtStatus);
            if (dataObject.IsGreen()) {
                Status.SetBackgroundColor(Color.Green);
            } else if (dataObject.IsRed()) {
                Status.SetBackgroundColor(Color.Red);
            } else {
                Status.SetBackgroundColor(Color.Yellow);
            }

            Status.Text = " ";



            return view; 
        }

    }
}