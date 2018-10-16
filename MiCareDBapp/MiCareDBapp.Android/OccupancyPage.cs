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
    [Activity(Label = "OccupancyPage", Theme = "@style/MainTheme")]
    public class OccupancyPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<OccupancyData> dataItems;
        private List<OccupancyData> displayItems;

        private int clickNumDate = 0;
        private int clickNumActualBeds = 0;
        private int clickNumSupported = 0;
        private int clickNumOccupancyRate = 0;
        private int clickNumTotalBedDays = 0;

        private OccupancyViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;
        private TextView TotalBedsValue;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.OccupancyPage, container, false);

            dataItems = new List<OccupancyData>();
            displayItems = new List<OccupancyData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new OccupancyViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextOccupancy);
            Button ActualBedsBtn = view.FindViewById<Button>(Resource.Id.ActualBedsTextOccupancy);
            Button OccupancyRateBtn = view.FindViewById<Button>(Resource.Id.OccupancyRateTextOccupancy);
            Button SupportedBtn = view.FindViewById<Button>(Resource.Id.SupportedTextOccupancy);
            Button TotalBedDaysBtn = view.FindViewById<Button>(Resource.Id.BedDaysTextOccupancy);

            //setup textview
            TotalBedsValue = view.FindViewById<TextView>(Resource.Id.TotalBedsValue);

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
                OccupancyGraph info = new OccupancyGraph(dataItems);
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
                values.Add("Type", "Occupancy");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<OccupancyData>>(json);
                    adapter = new OccupancyViewAdapter(this.Context, dataItems);
                    foreach (OccupancyData item in dataItems)
                    {
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
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumActualBeds = 0;
                    clickNumSupported = 0;
                    clickNumOccupancyRate = 0;
                    clickNumTotalBedDays = 0;
                    //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            ActualBedsBtn.Click += delegate {
                if (clickNumActualBeds == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetActualBeds().CompareTo(two.GetActualBeds());
                    });
                    clickNumActualBeds++;
                    clickNumDate = 0;
                    clickNumSupported = 0;
                    clickNumOccupancyRate = 0;
                    clickNumTotalBedDays = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumActualBeds = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            OccupancyRateBtn.Click += delegate {
                if (clickNumOccupancyRate == 0)
                {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetOccupancyRate().CompareTo(two.GetOccupancyRate());
                    });
                    clickNumOccupancyRate++;
                    clickNumActualBeds = 0;
                    clickNumDate = 0;
                    clickNumSupported = 0;
                    clickNumTotalBedDays = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumOccupancyRate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            SupportedBtn.Click += delegate {
                if (clickNumSupported == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetSupported().CompareTo(two.GetSupported());
                    });
                    clickNumSupported++;
                    clickNumActualBeds = 0;
                    clickNumDate = 0;
                    clickNumOccupancyRate = 0;
                    clickNumTotalBedDays = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumSupported = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            TotalBedDaysBtn.Click += delegate {
                if (clickNumTotalBedDays == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetTotalBedDays().CompareTo(two.GetTotalBedDays());
                    });
                    clickNumTotalBedDays++;
                    clickNumActualBeds = 0;
                    clickNumDate = 0;
                    clickNumOccupancyRate = 0;
                    clickNumSupported = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumTotalBedDays = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            int getOnce = 0;
            foreach (OccupancyData item in displayItems) {
                ID = item.GetFacilityID();
                if (ID == position) {
                    item.Show(false);
                    if (getOnce == 0) {
                        TotalBedsValue.Text = item.GetTotalBeds().ToString();
                        getOnce++;
                    }
                    if (!dataItems.Contains(item))
                    {
                        dataItems.Add(item);
                    }
                } else {
                    item.Show(true);
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
    }
}