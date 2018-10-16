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

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.AgencyUsagePage, container, false);

            dataItems = new List<AgencyUsageData>();
            displayItems = new List<AgencyUsageData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new AgencyUsageViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextAgency);
            Button AmountBtn = view.FindViewById<Button>(Resource.Id.AmountTextAgency);

            //setup Spinner
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.Clickable = false;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
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
                AgencyUsageGraph info = new AgencyUsageGraph(displayItems);
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
                values.Add("Type", "Agency");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<AgencyUsageData>>(json);
                    adapter = new AgencyUsageViewAdapter(this.Context, dataItems);//this
                    foreach (AgencyUsageData item in dataItems){
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

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            foreach (AgencyUsageData item in displayItems) {
                ID = item.GetFacilityID();
                if (ID == position) {
                    item.Show(false);
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                } else {
                    item.Show(true);
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
    }
}