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
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.BrokerageHoursPage, container, false);

            dataItems = new List<BrokerageHoursData>();
            displayItems = new List<BrokerageHoursData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new BrokerageHoursViewAdapter(view.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextBrokerage);
            Button HoursBtn = view.FindViewById<Button>(Resource.Id.HoursTextBrokerage);

            //setup Spinner
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.RegionSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.LocationArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/new2.php");

            toastMessage = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);

            //setup graph button
            Button GraphButton = view.FindViewById<Button>(Resource.Id.GraphButton);
            GraphButton.Enabled = false;
            GraphButton.Click += delegate {
                var transaction = ChildFragmentManager.BeginTransaction();
                BrokerageHoursGraph info = new BrokerageHoursGraph(displayItems);
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
                toastMessage.Show();
                spinner.SetSelection(0);
                dataItems.Clear();
                displayItems.Clear();
                spinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "Brokerage");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<BrokerageHoursData>>(json);
                    adapter = new BrokerageHoursViewAdapter(this.Context, dataItems);//this
                    foreach (BrokerageHoursData item in dataItems) {
                        displayItems.Add(item);
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

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            string itemLocationName;
            string locationName = "";
            if (position == 1) {
                locationName = "QLD";
            }
            else if (position == 2) {
                locationName = "VIC";
            }
            foreach (BrokerageHoursData item in displayItems) {
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

        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}