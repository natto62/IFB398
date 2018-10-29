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
    [Activity(Label = "ACFIPage", Theme = "@style/MainTheme")]
    public class ACFIPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<ACFIFunding> dataItems;
        private List<ACFIFunding> displayItems;
        private List<ACFIFunding> searchItems;

        private int clickNumResident = 0;
        private int clickNumScore = 0;
        private int clickNumIncome = 0;
        private int clickExpirationDate = 0;

        private ACFIViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;
        private TextView AvgIncome;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the ACFIPage.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.ACFIPage, container, false);

            //setup lists for acfi funding data 
            dataItems = new List<ACFIFunding>();
            displayItems = new List<ACFIFunding>();
            searchItems = new List<ACFIFunding>();

            //setup custom list adapter, more info found in ACFIViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new ACFIViewAdapter(this.Context, dataItems);

            //Display the number of data items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //AVG income per resident monthly textview
            AvgIncome = view.FindViewById<TextView>(Resource.Id.ACFIAvgValue);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button ResidentBtn = view.FindViewById<Button>(Resource.Id.ResidentIDTextACFI);
            Button ScoreBtn = view.FindViewById<Button>(Resource.Id.ScoreTextACFI);
            Button IncomeBtn = view.FindViewById<Button>(Resource.Id.IncomeTextACFI);
            Button ExpirationDateBtn = view.FindViewById<Button>(Resource.Id.ExpirationDateTextACFI);

            //setup Spinner to sort facilities
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.Clickable = false;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);//array found in Resources/values/
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //setup Month Spinner for the AVG income per resident monthly widget
            Spinner MonthSpinner = view.FindViewById<Spinner>(Resource.Id.MonthSpinner);
            MonthSpinner.Clickable = false;
            MonthSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_MonthItemSelected);
            var MonthSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.MonthArray, Android.Resource.Layout.SimpleSpinnerItem);
            MonthSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            MonthSpinner.Adapter = MonthSpinnerAdapter;

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
                ACFIGraph info = new ACFIGraph(displayItems);//displayItems to use all data
                info.Show(transaction, "dialog fragment");
            };

            //setup search bar
            SearchView SearchItems = view.FindViewById<SearchView>(Resource.Id.searchData);
            //an X apears next the search upon a submitted query, this X closes the current search, SearchCloseBtn button variable is associated with this
            //find the resource id's associated with the search and close buttons for the search bar widget
            int searchBtnID = SearchItems.Context.Resources.GetIdentifier("android:id/search_button", null, null);
            int closeBtnID = SearchItems.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
            var SearchOpenBtn = view.FindViewById<ImageView>(searchBtnID);
            var SearchCloseBtn = view.FindViewById<ImageView>(closeBtnID);
            SearchItems.SetIconifiedByDefault(false);//shows hint
            SearchItems.Enabled = false;//disable untill data is pulled
            SearchOpenBtn.Enabled = false;
            SearchItems.QueryTextSubmit += delegate {
                //this search bar allows the user to search via resident id
                string searchID = SearchItems.Query;
                foreach (ACFIFunding item in dataItems) {
                    //use searchItems list to hold all items while items get removed from the dataItems list if they don't match the search credentials
                    if (!searchItems.Contains(item)) {
                        searchItems.Add(item);
                    }
                }
                //if search credentials match or don't match, remove or add items from dataItems list
                foreach (ACFIFunding item in searchItems) {
                    if (dataItems.Contains(item) && (!String.Equals(item.GetResidentID().ToString(), searchID))) {
                        dataItems.Remove(item);
                    } else if (!dataItems.Contains(item) && (String.Equals(item.GetResidentID().ToString(), searchID))) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                adapter.NotifyDataSetChanged();
            };
            //when the search bar close button is pressed, add all items from searchItems into dataItems
            SearchCloseBtn.Click += delegate {
                foreach (ACFIFunding item in searchItems) {
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                SearchItems.SetQuery(String.Empty, false);
                adapter.NotifyDataSetChanged();
            };

            //setup progress bar for data pulling
            ProgressBar ClientProgress = view.FindViewById<ProgressBar>(Resource.Id.ClientProgress);
            //show progress percentage on the bar
            client.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e) {
                ClientProgress.Progress += e.ProgressPercentage;
            };

            //refresh button pulls data from database
            Button RefreshBtn = view.FindViewById<Button>(Resource.Id.RefreshButton);
            RefreshBtn.Click += delegate {
                RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIconClicked);//change refresh icon colour to lighter shade of green
                toastMessage.Show();//show toast message
                spinner.SetSelection(0);
                //clear lists, to make way for updated data
                dataItems.Clear();
                displayItems.Clear();
                spinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "ACFI");
                //call php file and use UploadValuesAsync with the value of Type=ACFI so the php file knows to pull ACFI data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<ACFIFunding>>(json);//use json to create a list of data objects from the output of the php file
                    int[] residentIDArray = new int[dataItems.Count];
                    int count = 0;
                    //put all resident id values into an array
                    foreach (ACFIFunding item in dataItems)
                    {
                        residentIDArray[count] = item.GetResidentID();
                        count++;
                        displayItems.Add(item);//display items holds all of the data objects for safe keeping, for when dataItems objects get removed
                    }
                    //get distinct resident id values in its own array
                    var distinctResidentIDvals = residentIDArray.Distinct();
                    bool isGreen = false;
                    bool isRed = false;
                    decimal sum = 0;
                    //foreach distinct resident id value check if the rows for a resident id should be green, yellow or red, this is beacuse some resident id's correspond with multiple rows
                    foreach (int ID in distinctResidentIDvals) {
                        sum = 0;
                        foreach (ACFIFunding item in dataItems.Where(x => x.GetResidentID() == ID)) {
                            sum = sum + item.GetIncome();
                        }
                        //if the sum of all acfi income for a resident is above $200
                        if (sum > 200) {
                            isGreen = true;
                            isRed = false;
                        } else if (sum < 150)
                        { //if the sum of all acfi income for a resident is below $150
                            isGreen = false;
                            isRed = true;
                        } else {//yellow
                            isGreen = false;
                            isRed = false;
                        }
                        foreach (ACFIFunding item in dataItems.Where(x => x.GetResidentID() == ID)) {
                            item.SetGreen(isGreen);
                            item.SetRed(isRed);
                        }
                    }
                    adapter = new ACFIViewAdapter(this.Context, dataItems);//setup adapter
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    spinner.Clickable = true;
                    GraphButton.Enabled = true;
                    SearchItems.Enabled = true;
                    SearchOpenBtn.Enabled = true;
                    toastMessage.Cancel();
                    searchItems.Clear();
                    adapter.NotifyDataSetChanged();
                });
            };

            //sort the items based on residentID
            ResidentBtn.Click += delegate {
                if (clickNumResident == 0)
                {
                    dataItems.Sort(delegate (ACFIFunding one, ACFIFunding two) {
                        return one.GetResidentID().CompareTo(two.GetResidentID());
                    });
                    clickNumResident++;
                    clickNumScore = 0;
                    clickNumIncome = 0;
                    clickExpirationDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumResident = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //sort the items based on score
            ScoreBtn.Click += delegate {
                if (clickNumScore == 0)
                {
                    dataItems.Sort(delegate (ACFIFunding one, ACFIFunding two) {
                        return one.GetACFIScore().CompareTo(two.GetACFIScore());
                    });
                    clickNumScore++;
                    clickNumResident = 0;
                    clickNumIncome = 0;
                    clickExpirationDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumScore = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //sort the items based on income
            IncomeBtn.Click += delegate {
                if (clickNumIncome == 0)
                {
                    dataItems.Sort(delegate (ACFIFunding one, ACFIFunding two) {
                        return one.GetIncome().CompareTo(two.GetIncome());
                    });
                    clickNumIncome++;
                    clickNumResident = 0;
                    clickNumScore = 0;
                    clickExpirationDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumIncome = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //sport the items based on expiration date
            ExpirationDateBtn.Click += delegate {
                if (clickExpirationDate == 0) {
                    dataItems.Sort(delegate (ACFIFunding one, ACFIFunding two) {
                        return DateTime.Compare(one.GetExpirationDate(), two.GetExpirationDate());
                    });
                    clickExpirationDate++;
                    clickNumResident = 0;
                    clickNumScore = 0;
                    clickNumIncome = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickExpirationDate = 0;
                }
                adapter.NotifyDataSetChanged();

            };

            return view;
        }

        //the spinner for selecting months for the AVG income per resident monthly textview
        private void Spinner_MonthItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;//get spinner position
            if (position == 0) {//if default position, no month selected
                AvgIncome.Text = "";
            } else { 
                decimal totalAmount = 0;
                decimal avgValue = 0;
                decimal numItems = 0;
                foreach (ACFIFunding item in displayItems) {
                    if (position == item.GetDate().Month) {
                        numItems = numItems + 1;
                        totalAmount = totalAmount + item.GetIncome();
                    }
                }
                if (numItems != 0) {
                    avgValue = totalAmount / numItems;
                }
                AvgIncome.Text = " $ " + avgValue.ToString();
            }
        }
        //the spinner for selecting a facility
        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;//get spinner position
            searchItems.Clear();
            foreach (ACFIFunding item in displayItems)
            {//foreach data item, add or remove depending on if it is associated with the chosen facility
                ID = item.GetFacilityID();
                if (ID == position)
                {
                    if (!dataItems.Contains(item))
                    {
                        dataItems.Add(item);
                    }
                }
                else
                {
                    if (position > 0)
                    {
                        dataItems.Remove(item);
                    }
                    else
                    {
                        if (!dataItems.Contains(item))
                        {
                            dataItems.Add(item);
                        }
                    }

                }
            }
            adapter.NotifyDataSetChanged();
        }
        //a public notify adapter method which is used in the Settings.cs file to update all fragment kpi's depending on changed settings, eg. text size
        public void NotifyAdapter() {
            adapter.NotifyDataSetChanged();
        }
    }
}