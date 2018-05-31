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
        private HomeCarePackageData dataObject;

        private TextView FName;
        private TextView LName;
        private TextView Income;
        private TextView Level;

        //import the finance data item based on the item clicked 
        public MoreInfo(HomeCarePackageData data) {
            dataObject = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DataPage2, container, false);


            //change the text based on the item clicked on the list
            FName = view.FindViewById<TextView>(Resource.Id.txtFName);
            FName.Text = dataObject.GetResidentFirstName();

            LName = view.FindViewById<TextView>(Resource.Id.txtLName);
            LName.Text = dataObject.GetResidentLastName();

            Level = view.FindViewById<TextView>(Resource.Id.txtLevel);
            Level.Text = dataObject.GetPackageLevel().ToString();

            Income = view.FindViewById<TextView>(Resource.Id.txtIncome);
            Income.Text = "$ " + dataObject.GetPackageIncome().ToString();

            if (dataObject.IsGreen()){
                view.SetBackgroundColor(Color.LightGreen);
            } else if (dataObject.IsRed()){
                view.SetBackgroundColor(Color.Argb(80, 255, 128, 128));
            } else {
                view.SetBackgroundColor(Color.LightYellow);
            }

            return view; 
        }

    }
}