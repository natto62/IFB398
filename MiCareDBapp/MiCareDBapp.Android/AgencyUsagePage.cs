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
    [Activity(Label = "AgencyUsagePage", Theme = "@style/MainTheme")]
    public class AgencyUsagePage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<AgencyUsageData> dataItems;
        private List<AgencyUsageData> displayItems;

        private int clickNumDate = 0;
        private int clickNumAmount = 0;

        private AgencyUsageViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;
        private int facilityForGraph = 0;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the AgencyUsagePage.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.AgencyUsagePage, container, false);

            //setup lists for agency usage data 
            dataItems = new List<AgencyUsageData>();
            displayItems = new List<AgencyUsageData>();

            //setup custom list adapter, more info found in AgencyUsageViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new AgencyUsageViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextAgency);
            Button AmountBtn = view.FindViewById<Button>(Resource.Id.AmountTextAgency);

            //setup Spinner to sort facilities
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.Clickable = false;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
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
                if (facilityForGraph > 0) {//only create graph if facility has been chosen
                    var transaction = ChildFragmentManager.BeginTransaction();
                    AgencyUsageGraph info = new AgencyUsageGraph(dataItems, facilityForGraph);
                    info.Show(transaction, "dialog fragment");
                } else {
                    Toast FacilityIDMessage = Toast.MakeText(this.Context, "Please select a facility", ToastLength.Long);
                    FacilityIDMessage.Show();
                }
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
                values.Add("Type", "Agency");
                //call php file and use UploadValuesAsync with the value of Type=Agency so the php file knows to pull agency usage data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<AgencyUsageData>>(json);//use json to create a list of data objects from the output of the php file
                    adapter = new AgencyUsageViewAdapter(this.Context, dataItems);//setup adapter
                    foreach (AgencyUsageData item in dataItems){
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
                if (clickNumDate == 0) {
                    dataItems.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumAmount = 0;
                    //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };
            //sort items based on amount
            AmountBtn.Click += delegate {
                if (clickNumAmount == 0) {
                    dataItems.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                        return one.GetAgencyUsageAmount().CompareTo(two.GetAgencyUsageAmount());
                    });
                    clickNumAmount++;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumAmount = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }
        //the spinner for selecting a facility
        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;//get spinner position
            facilityForGraph = position;
            foreach (AgencyUsageData item in displayItems)
            {//foreach data item, add or remove depending on if it is associated with the chosen facility
                ID = item.GetFacilityID();
                if (ID == position) {
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                } else {
                    if (position > 0) {
                        dataItems.Remove(item);
                    } else {
                        if (!dataItems.Contains(item)) {
                            dataItems.Add(item);
                        }
                    }
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