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
    [Activity(Label = "ItemPage", Theme = "@style/MainTheme")]
    public class HomeCarePackagePage : Android.Support.V4.App.Fragment
    {
        //int number;

        private ListView dataList;
        private List<HomeCarePackageData> dataItems;
        private List<HomeCarePackageData> displayItems;
        private List<HomeCarePackageData> searchItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumIncome = 0;
        private int clcikNumLevel = 0;

        private HomeCarePackageViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the HomeCarePackagePage.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.HomeCarePackagePage, container, false);

            //setup lists for home care package data
            dataItems = new List<HomeCarePackageData>();
            displayItems = new List<HomeCarePackageData>();
            searchItems = new List<HomeCarePackageData>();

            //setup custom list adapter, more info found in HomeCarePackageViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new HomeCarePackageViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = view.FindViewById<Button>(Resource.Id.FirstNameTextHomeCare);
            Button LName = view.FindViewById<Button>(Resource.Id.LastNameTextHomeCare);
            Button PackageLevel = view.FindViewById<Button>(Resource.Id.PackageLevelText);
            Button IncomeList = view.FindViewById<Button>(Resource.Id.PackageIncomeText);

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
                var transaction = ChildFragmentManager.BeginTransaction();
                HomeCarePackagesGraph info = new HomeCarePackagesGraph(displayItems);//displayItems to use all data
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
                //this search bar allows the user to search via first and/or last name 
                string searchName = SearchItems.Query;
                //Get same string with first letter uppercase
                char firstUpper = searchName[0];
                firstUpper = Char.ToUpper(firstUpper);
                char[] searchNameLetters = searchName.ToCharArray();
                searchNameLetters[0] = firstUpper;
                string searchNameUpper = new string(searchNameLetters);

                foreach (HomeCarePackageData item in dataItems) {
                    //use searchItems list to hold all items while items get removed from the dataItems list if they don't match the search credentials
                    if (!searchItems.Contains(item)) {
                        searchItems.Add(item);
                    }
                }
                //if search credentials match or don't match, remove or add items from dataItems list
                foreach (HomeCarePackageData item in searchItems) {
                    bool foundFName = String.Equals(searchName, item.GetResidentFirstName(), StringComparison.OrdinalIgnoreCase);//ignores upper or lower case
                    bool foundLName = String.Equals(searchName, item.GetResidentLastName(), StringComparison.OrdinalIgnoreCase);
                    bool insideName = item.GetResidentFirstName().Contains(searchName) || item.GetResidentLastName().Contains(searchName);
                    bool insideNameUpper = item.GetResidentFirstName().Contains(searchNameUpper) || item.GetResidentLastName().Contains(searchNameUpper);
                    if (dataItems.Contains(item) && !foundFName && !foundLName && !insideName && !insideNameUpper) {
                        dataItems.Remove(item);
                    } else if (!dataItems.Contains(item) && (foundFName || foundLName || insideName || insideNameUpper)) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                adapter.NotifyDataSetChanged();
            };
            //when the search bar close button is pressed, add all items from searchItems into dataItems
            SearchCloseBtn.Click += delegate {
                foreach (HomeCarePackageData item in searchItems) {
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                SearchItems.SetQuery(String.Empty, false);
                adapter.NotifyDataSetChanged();
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
                values.Add("Type", "Home");
                //call php file and use UploadValuesAsync with the value of Type=Home so the php file knows to pull home care package data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<HomeCarePackageData>>(json);//use json to create a list of data objects from the output of the php file
                    adapter = new HomeCarePackageViewAdapter(this.Context, dataItems);//setup adapter
                    foreach (HomeCarePackageData item in dataItems) {
                        displayItems.Add(item);//display items holds all of the data objects for safe keeping, for when dataItems objects get removed
                    }
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

            //setup button for sorting the list based on first names
            FName.Click += delegate {
                if (clickNumFName == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return string.Compare(one.GetResidentFirstName(), two.GetResidentFirstName());
                    });
                    clickNumFName++;
                    clickNumLName = 0;
                    clickNumIncome = 0;
                    clcikNumLevel = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumFName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on last names
            LName.Click += delegate {
                if (clickNumLName == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return string.Compare(one.GetResidentLastName(), two.GetResidentLastName());
                    });
                    clickNumLName++;
                    clickNumFName = 0;
                    clickNumIncome = 0;
                    clcikNumLevel = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumLName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on package levels
            PackageLevel.Click += delegate {
                if (clcikNumLevel == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return one.GetPackageLevel().CompareTo(two.GetPackageLevel());
                    });
                    clcikNumLevel++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumIncome = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clcikNumLevel = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on incomes
            IncomeList.Click += delegate {
                if (clickNumIncome == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return one.GetPackageIncome().CompareTo(two.GetPackageIncome());
                    });
                    clickNumIncome++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clcikNumLevel = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumIncome = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }
        //the spinner for selecting a facility
        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int ID;
            searchItems.Clear();
            int position = e.Position;//get spinner position
            foreach (HomeCarePackageData item in displayItems) {
                ID = item.GetFacilityID();
                if (ID == position)
                {//foreach data item, add or remove depending on if it is associated with the chosen facility
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
        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}