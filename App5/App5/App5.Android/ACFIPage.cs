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

namespace MiCareApp.Droid
{
    [Activity(Label = "ACFIPage", Theme = "@style/MainTheme")]
    public class ACFIPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<ACFIFunding> dataItems;
        private List<ACFIFunding> displayItems;

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
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.ACFIPage, container, false);

            dataItems = new List<ACFIFunding>();
            displayItems = new List<ACFIFunding>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new ACFIViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumACFIData);

            //AVG income per resident monthly
            AvgIncome = view.FindViewById<TextView>(Resource.Id.ACFIAvgValue);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button ResidentBtn = view.FindViewById<Button>(Resource.Id.ResidentIDTextACFI);
            Button ScoreBtn = view.FindViewById<Button>(Resource.Id.ScoreTextACFI);
            Button IncomeBtn = view.FindViewById<Button>(Resource.Id.IncomeTextACFI);
            Button ExpirationDateBtn = view.FindViewById<Button>(Resource.Id.ExpirationDateTextACFI);

            //setup Spinner
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.Clickable = false;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //setup Month Spinner
            Spinner MonthSpinner = view.FindViewById<Spinner>(Resource.Id.MonthSpinner);
            MonthSpinner.Clickable = false;
            MonthSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_MonthItemSelected);
            var MonthSpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.MonthArray, Android.Resource.Layout.SimpleSpinnerItem);
            MonthSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            MonthSpinner.Adapter = MonthSpinnerAdapter;

            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/new2.php");

            toastMessage = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);

            Button RefreshBtn = view.FindViewById<Button>(Resource.Id.RefreshButton);
            RefreshBtn.Click += delegate {
                RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIconClicked);
                toastMessage.Show();
                spinner.SetSelection(0);
                dataItems.Clear();
                displayItems.Clear();
                spinner.Clickable = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "ACFI");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<ACFIFunding>>(json);
                    adapter = new ACFIViewAdapter(this.Context, dataItems);//this
                    foreach (ACFIFunding item in dataItems)
                    {
                        displayItems.Add(item);
                    }
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    spinner.Clickable = true;
                    toastMessage.Cancel();
                    adapter.NotifyDataSetChanged();
                });
            };

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

        private void Spinner_MonthItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int position = e.Position;
            if (position == 0) {
                AvgIncome.Text = "";
            } else { 
                decimal totalAmount = 0;
                decimal numItems = 0;
                foreach (ACFIFunding item in displayItems) {
                    if (position == item.GetDate().Month) {
                        numItems = numItems + 1;
                        totalAmount = totalAmount + item.GetIncome();
                    }
                }
                decimal avgValue = totalAmount/numItems;
                AvgIncome.Text = "$ " + avgValue.ToString();
            }
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            foreach (ACFIFunding item in displayItems)
            {
                ID = item.GetFacilityID();
                if (ID == position)
                {
                    item.Show(false);
                    if (!dataItems.Contains(item))
                    {
                        dataItems.Add(item);
                    }
                }
                else
                {
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