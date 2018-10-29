using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "BrokerageHoursPage", Theme = "@style/MainTheme")]
    public class BrokerageHoursPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<BrokerageHoursData> dataItems;
        private List<BrokerageHoursData> displayItems;

        private int clickNumDate = 0;
        private int clickNumHours = 0;

        private BrokerageHoursViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the BrokerageHours.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.BrokerageHoursPage, container, false);

            //setup lists for brokerage hours data 
            dataItems = new List<BrokerageHoursData>();
            displayItems = new List<BrokerageHoursData>();

            //setup custom list adapter, more info found in BrokerageHoursViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new BrokerageHoursViewAdapter(view.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextBrokerage);
            Button HoursBtn = view.FindViewById<Button>(Resource.Id.HoursTextBrokerage);

            //setup Spinner to sort location
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.RegionSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.LocationArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //create new WebClient class object which can provide methods to push and recieve data from an online resource via a url
            client = new WebClient();
            //set the url to push and pull data from, via the a Uri class object
            //the online resource is a php file hosted on heroku, these php files read write and pull database tables
            url = new Uri("https://capstonephpcode198.herokuapp.com/PullData.php");

            //setup toast message which is pop up message which informs the user that data is being pulled
            toastMessage = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);

            //setup graph button
            Button GraphButton = view.FindViewById<Button>(Resource.Id.GraphButton);
            GraphButton.Enabled = false;//disabled untill data is pulled
            GraphButton.Click += delegate {
                var transaction = ChildFragmentManager.BeginTransaction();
                BrokerageHoursGraph info = new BrokerageHoursGraph(displayItems);//displayItems to use all data
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
                toastMessage.Show();
                spinner.SetSelection(0);
                //clear lists, to make way for updated data
                dataItems.Clear();
                displayItems.Clear();
                spinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "Brokerage");
                //call php file and use UploadValuesAsync with the value of Type=Brokerage so the php file knows to pull brokerage usage data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<BrokerageHoursData>>(json);//use json to create a list of data objects from the output of the php file
                    adapter = new BrokerageHoursViewAdapter(this.Context, dataItems);//setup adapter
                    foreach (BrokerageHoursData item in dataItems) {
                        displayItems.Add(item);//display items holds all of the data objects for safe keeping, for when dataItems objects get removed
                    }
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    spinner.Clickable = true;
                    GraphButton.Enabled = true;
                    toastMessage.Cancel();
                    adapter.NotifyDataSetChanged();
                });
            };
            //sort items based on date
            DateBtn.Click += delegate {
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (BrokerageHoursData one, BrokerageHoursData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumHours = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };
            //sort items based on brokerage hours
            HoursBtn.Click += delegate {
                if (clickNumHours == 0)
                {
                    dataItems.Sort(delegate (BrokerageHoursData one, BrokerageHoursData two) {
                        return one.GetBrokerageHours().CompareTo(two.GetBrokerageHours());
                    });
                    clickNumHours++;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumHours = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }
        //the spinner for selecting a location
        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;//get spinner position
            string itemLocationName;
            string locationName = "";
            if (position == 1) {
                locationName = "QLD";
            }
            else if (position == 2) {
                locationName = "VIC";
            }
            foreach (BrokerageHoursData item in displayItems)
            {//foreach data item, add or remove depending on if it is associated with the chosen location
                itemLocationName = item.GetLocation();
                if (String.Equals(locationName, itemLocationName)) {
                    if (!dataItems.Contains(item))
                    {
                        dataItems.Add(item);
                    }
                } else {
                    dataItems.Remove(item);
                }
            }
            adapter.NotifyDataSetChanged();
        }
        //a public notify adapter method which is used in the Settings.cs file to update all fragment kpi's depending on changed settings, eg. text size
        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}