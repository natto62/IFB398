﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Specialized;

namespace MiCareApp.Droid
{
    [Activity(Label = "IncomePage", Theme = "@style/MainTheme")]
    public class IncomePage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<IncomeData> dataItems;
        private List<IncomeData> displayItems;

        private int clickNumDate = 0;
        private int clickNumIncome = 0;
        private string type;

        private IncomeViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessageRefresh;
        private Toast toastMessagePulled;
        private Toast toastMessageFilter;

        private int [] spinnerPositions = new int[3] { 0, 0, 0 };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.IncomePage, container, false);

            dataItems = new List<IncomeData>();
            displayItems = new List<IncomeData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new IncomeViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumIncomeData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextIncome);
            Button IncomeBtn = view.FindViewById<Button>(Resource.Id.IncomeTextIncome);
            IncomeBtn.Enabled = false;
            DateBtn.Enabled = false;

            Button FilterBtn = view.FindViewById<Button>(Resource.Id.FilterButton);

            //setup Location Spinner
            Spinner LocationSpinner = view.FindViewById<Spinner>(Resource.Id.LocationSpinner);
            LocationSpinner.Clickable = false;
            LocationSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_LocationItemSelected);
            var LocationSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.LocationArray, Android.Resource.Layout.SimpleSpinnerItem);
            LocationSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            LocationSpinner.Adapter = LocationSpinnerAdapter;

            //setup Business Class Spinner
            Spinner BusinessClassSpinner = view.FindViewById<Spinner>(Resource.Id.BusinessClassSpinner);
            BusinessClassSpinner.Clickable = false;
            BusinessClassSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_BusinessClassItemSelected);
            var BusinessClassSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.BusinessClassArray, Android.Resource.Layout.SimpleSpinnerItem);
            BusinessClassSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            BusinessClassSpinner.Adapter = BusinessClassSpinnerAdapter;

            //setup Income Type Spinner
            Spinner IncomeTypeSpinner = view.FindViewById<Spinner>(Resource.Id.IncomeTypeSpinner);
            IncomeTypeSpinner.Clickable = false;
            IncomeTypeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_IncomeTypeItemSelected);
            var IncomeTypeSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.IncomeTypeArray, Android.Resource.Layout.SimpleSpinnerItem);
            IncomeTypeSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            IncomeTypeSpinner.Adapter = IncomeTypeSpinnerAdapter;

            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/new2.php");

            toastMessageRefresh = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);
            toastMessagePulled = Toast.MakeText(this.Context, "Data has been pulled", ToastLength.Long);
            toastMessageFilter = Toast.MakeText(this.Context, "An option must be selected from all dropdown boxes", ToastLength.Long);

            Button RefreshBtn = view.FindViewById<Button>(Resource.Id.RefreshButton);
            RefreshBtn.Click += delegate {
                RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIconClicked);
                toastMessageRefresh.Show();
                LocationSpinner.SetSelection(0);
                BusinessClassSpinner.SetSelection(0);
                IncomeTypeSpinner.SetSelection(0);
                dataItems.Clear();
                displayItems.Clear();
                LocationSpinner.Clickable = false;
                BusinessClassSpinner.Clickable = false;
                IncomeTypeSpinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "Income");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<IncomeData>>(json);
                    adapter = new IncomeViewAdapter(this.Context, dataItems);//this
                    foreach (IncomeData item in dataItems) {
                        displayItems.Add(item);
                        dataItems.Remove(item);
                    }
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    LocationSpinner.Clickable = true;
                    BusinessClassSpinner.Clickable = true;
                    IncomeTypeSpinner.Clickable = true;
                    toastMessageRefresh.Cancel();
                    toastMessagePulled.Show();
                });
            };

            DateBtn.Click += delegate {
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (IncomeData one, IncomeData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumIncome = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            IncomeBtn.Click += delegate {
                if (clickNumIncome == 0)
                {
                    dataItems.Sort(delegate (IncomeData one, IncomeData two) {
                        return one.GetIncome().CompareTo(two.GetIncome());
                    });
                    clickNumIncome++;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumIncome = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            FilterBtn.Click += delegate{
                bool canBeFiltered = true;
                for (int i = 0; i < 3; i++) {
                    if (spinnerPositions[i] == 0) {
                        canBeFiltered = false;
                    }
                }
                if (canBeFiltered) {
                    adapter.NotifyDataSetChanged();
                } else {
                    toastMessageFilter.Show();
                }
            };

            return view;
        }

        void Spinner_IncomeTypeItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[0] = position;
            string type = "";
            if (position == 1) {
                type = "Total Package Income";
            } else if (position == 2) {
                type = "Business Service";
            } else if (position == 3) {
                type = "Settlement Service";
            }
            foreach (IncomeData item in displayItems) {
                item.SetType(type);
            }

        }

        void Spinner_BusinessClassItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[1] = position;
            string itemType = "";
            string type = "";
            if (position == 1) {
                type = "Residential";
            } else if (position == 2) {
                type = "Rural";
            }
            foreach (IncomeData item in displayItems) {
                if (item.GetFacilityID() == 1) {
                    itemType = "Rural";
                } else if (item.GetFacilityID() > 1) {
                    itemType = "Residential";
                }
                if (String.Equals(itemType, type)) {
                    if (!dataItems.Contains(item))
                    {
                        dataItems.Add(item);
                    }
                } else {
                    dataItems.Remove(item);
                }
            }
        }

        void Spinner_LocationItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[2] = position;
            string itemLocationName;
            string locationName = "";
            if (position == 1) {
                locationName = "QLD";
            } else if (position == 2) {
                locationName = "VIC";
            }
            foreach (IncomeData item in displayItems) {
                itemLocationName = item.GetLocation();
                if (String.Equals(locationName, itemLocationName)) {
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                } else {
                    dataItems.Remove(item);
                }
            }
        }
    }
}