using System;
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
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "IncomePage", Theme = "@style/MainTheme")]
    public class IncomePage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<IncomeData> dataItems;
        private List<IncomeData> displayItems;

        private int clickNumDate = 0;
        private int clickNumIncome = 0;
        private string locationName = "";
        private string type = "";

        private IncomeViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;
        private Button FilterBtn;

        private Toast toastMessageRefresh;
        private Toast toastMessagePulled;
        private Toast toastMessageFilter;

        private int [] spinnerPositions = new int[3] { 0, 0, 0 };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {   //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the IncomePage.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.IncomePage, container, false);

            //setup lists for income data
            dataItems = new List<IncomeData>();
            displayItems = new List<IncomeData>();

            //setup custom list adapter, more info found in IncomeViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new IncomeViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextIncome);
            Button IncomeBtn = view.FindViewById<Button>(Resource.Id.IncomeTextIncome);
            IncomeBtn.Enabled = false;
            DateBtn.Enabled = false;

            //setup filter button
            FilterBtn = view.FindViewById<Button>(Resource.Id.FilterButton);

            //setup Location Spinner
            Spinner LocationSpinner = view.FindViewById<Spinner>(Resource.Id.LocationSpinner);
            LocationSpinner.Clickable = false;
            LocationSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_LocationItemSelected);
            var LocationSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.LocationArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            LocationSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            LocationSpinner.Adapter = LocationSpinnerAdapter;

            //setup Business Class Spinner
            Spinner BusinessClassSpinner = view.FindViewById<Spinner>(Resource.Id.BusinessClassSpinner);
            BusinessClassSpinner.Clickable = false;
            BusinessClassSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_BusinessClassItemSelected);
            var BusinessClassSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.BusinessClassArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            BusinessClassSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            BusinessClassSpinner.Adapter = BusinessClassSpinnerAdapter;

            //setup Income Type Spinner
            Spinner IncomeTypeSpinner = view.FindViewById<Spinner>(Resource.Id.IncomeTypeSpinner);
            IncomeTypeSpinner.Clickable = false;
            IncomeTypeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_IncomeTypeItemSelected);
            var IncomeTypeSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.IncomeTypeArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            IncomeTypeSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            IncomeTypeSpinner.Adapter = IncomeTypeSpinnerAdapter;

            //create new WebClient class object which can provide methods to push and recieve data from an online resource via a url
            client = new WebClient();
            //set the url to push and pull data from, via the a Uri class object
            //the online resource is a php file hosted on heroku, these php files read write and pull database tables
            url = new Uri("https://capstonephpcode198.herokuapp.com/PullData.php");

            //setup toast message which is pop up message which informs the user that data is being pulled
            toastMessageRefresh = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);
            //setup toast message which is pop up message which informs the user that data has been pulled
            toastMessagePulled = Toast.MakeText(this.Context, "Data has been pulled", ToastLength.Long);
            //setup toast message which is pop up message which informs the user that an option from all the spinners must be selected
            toastMessageFilter = Toast.MakeText(this.Context, "An option must be selected from all dropdown boxes", ToastLength.Long);

            //setup graph button
            Button GraphButton = view.FindViewById<Button>(Resource.Id.GraphButton);
            GraphButton.Enabled = false;//disabled untill data is pulled
            GraphButton.Click += delegate {
                var transaction = ChildFragmentManager.BeginTransaction();
                IncomeGraph info = new IncomeGraph(dataItems);
                info.Show(transaction, "dialog fragment");
            };

            //setup progress bar
            ProgressBar ClientProgress = view.FindViewById<ProgressBar>(Resource.Id.ClientProgress);
            //show progress percentage on the bar
            client.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e) {
                ClientProgress.Progress += e.ProgressPercentage;
            };
            //refresh button pulls data from database
            Button RefreshBtn = view.FindViewById<Button>(Resource.Id.RefreshButton);
            RefreshBtn.Click += delegate {
                RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIconClicked);//change refresh icon colour to lighter shade of green
                toastMessageRefresh.Show();
                //set all spinners to default position
                LocationSpinner.SetSelection(0);
                BusinessClassSpinner.SetSelection(0);
                IncomeTypeSpinner.SetSelection(0);
                //clear lists, to make way for updated data
                dataItems.Clear();
                displayItems.Clear();
                LocationSpinner.Clickable = false;
                BusinessClassSpinner.Clickable = false;
                IncomeTypeSpinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "Income");
                //call php file and use UploadValuesAsync with the value of Type=Income so the php file knows to pull income data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<IncomeData>>(json);//use json to create a list of data objects from the output of the php file
                    adapter = new IncomeViewAdapter(this.Context, dataItems);//setup adapter
                    foreach (IncomeData item in dataItems) {
                        displayItems.Add(item);//display items holds all of the data objects for safe keeping, for when dataItems objects get removed
                    }
                    foreach (IncomeData item in displayItems) {
                        dataItems.Remove(item);//remove dataItems initially so that the list view is empty until the user has filtered the data via the spinners
                    }
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    LocationSpinner.Clickable = true;
                    BusinessClassSpinner.Clickable = true;
                    IncomeTypeSpinner.Clickable = true;
                    GraphButton.Enabled = true;
                    toastMessageRefresh.Cancel();
                    toastMessagePulled.Show();
                });
            };
            //sort data via date
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
            //sort data via income
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
            //filter button can be clicked once the checkIfFilter() method allows it
            FilterBtn.Click += delegate{
                string itemLocationName;
                string itemType = "";
                foreach (IncomeData item in displayItems) {
                    if (item.GetFacilityID() > 0 && item.GetFacilityID() < 3) {
                        itemType = "Rural";
                    } else if (item.GetFacilityID() > 2) {
                        itemType = "Residential";
                    }
                    itemLocationName = item.GetLocation();
                    //if the local variables match or do not match the global variables set via the spinners, add or remove items from list
                    if (String.Equals(locationName, itemLocationName) && String.Equals(itemType, type)) {
                        if (!dataItems.Contains(item)) {
                            dataItems.Add(item);
                        }
                    } else {
                        dataItems.Remove(item);
                    }
                }
                adapter.NotifyDataSetChanged();
                IncomeBtn.Enabled = true;
                DateBtn.Enabled = true;
            };

            return view;
        }
        //spinner to select income type, sets the type of income to display for each income item, look at GetIncome() in Data.cs for more
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
            checkIfFilter();//check if all spinner have a selected option
        }
        //spinner to select business class, assign to global variable "type"
        void Spinner_BusinessClassItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[1] = position;
            if (position == 1) {
                type = "Residential";
            } else if (position == 2) {
                type = "Rural";
            }
            checkIfFilter();//check if all spinner have a selected option
        }
        //spinner to select location, assign to global variable "locationName"
        void Spinner_LocationItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[2] = position;
            if (position == 1) {
                locationName = "QLD";
            } else if (position == 2) {
                locationName = "VIC";
            }
            checkIfFilter();//check if all spinner have a selected option
        }
        //check if all spinner have a selected option
        void checkIfFilter() {
            bool canBeFiltered = true;
            for (int i = 0; i < 3; i++) {
                if (spinnerPositions[i] == 0) {
                    canBeFiltered = false;
                }
            }
            if (canBeFiltered) {
                FilterBtn.Enabled = true;
            } else {
                FilterBtn.Enabled = false;
            }
        }

        //a public notify adapter method which is used in the Settings.cs file to update all fragment kpi's depending on changed settings, eg. text size
        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}