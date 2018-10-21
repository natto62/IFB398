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
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.IncomePage, container, false);

            dataItems = new List<IncomeData>();
            displayItems = new List<IncomeData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new IncomeViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextIncome);
            Button IncomeBtn = view.FindViewById<Button>(Resource.Id.IncomeTextIncome);
            IncomeBtn.Enabled = false;
            DateBtn.Enabled = false;

            FilterBtn = view.FindViewById<Button>(Resource.Id.FilterButton);

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

            //setup graph button
            Button GraphButton = view.FindViewById<Button>(Resource.Id.GraphButton);
            GraphButton.Enabled = false;
            GraphButton.Click += delegate {
                var transaction = ChildFragmentManager.BeginTransaction();
                IncomeGraph info = new IncomeGraph(dataItems);
                info.Show(transaction, "dialog fragment");
            };

            //setup progress bar
            ProgressBar ClientProgress = view.FindViewById<ProgressBar>(Resource.Id.ClientProgress);

            client.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e) {
                ClientProgress.Progress += e.ProgressPercentage;
            };

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
                    }
                    foreach (IncomeData item in displayItems) {
                        dataItems.Remove(item);
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
                string itemLocationName;
                string itemType = "";
                foreach (IncomeData item in displayItems) {
                    if (item.GetFacilityID() > 0 && item.GetFacilityID() < 3) {
                        itemType = "Rural";
                    } else if (item.GetFacilityID() > 2) {
                        itemType = "Residential";
                    }
                    itemLocationName = item.GetLocation();
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
            checkIfFilter();
        }

        void Spinner_BusinessClassItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[1] = position;
            if (position == 1) {
                type = "Residential";
            } else if (position == 2) {
                type = "Rural";
            }
            checkIfFilter();
        }

        void Spinner_LocationItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            spinnerPositions[2] = position;
            if (position == 1) {
                locationName = "QLD";
            } else if (position == 2) {
                locationName = "VIC";
            }
            checkIfFilter();
        }

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

        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}