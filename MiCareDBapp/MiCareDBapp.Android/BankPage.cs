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
using System.Globalization;
using Android;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "BankPage", Theme = "@style/MainTheme")]
    public class BankPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<BankBalance> dataItems;
        private List<BankBalance> displayItems;

        private int clickNumDate = 0;
        private int clickNumBalance = 0;

        private BankViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;
        private TextView BankBalanceView;

        private Button GraphButton;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.BankPage, container, false);

            dataItems = new List<BankBalance>();
            displayItems = new List<BankBalance>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new BankViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextBank);
            Button BalanceBtn = view.FindViewById<Button>(Resource.Id.BalanceTextBank);

            //setup bank balance textview
            BankBalanceView = view.FindViewById<TextView>(Resource.Id.BankBalanceValue);

            //setup progress bar
            ProgressBar ClientProgress = view.FindViewById<ProgressBar>(Resource.Id.ClientProgress);

            //setup graph button
            GraphButton = view.FindViewById<Button>(Resource.Id.GraphButton);
            GraphButton.Enabled = false;
            GraphButton.Click += delegate {
                var transaction = ChildFragmentManager.BeginTransaction();
                BankBalanceGraph info = new BankBalanceGraph(dataItems);
                info.Show(transaction, "dialog fragment");
            };

            client = new WebClient();
            url = new Uri("https://capstonephpcode198.herokuapp.com/new2.php");

            toastMessage = Toast.MakeText(this.Context, "Fetching data", ToastLength.Long);

            Button RefreshBtn = view.FindViewById<Button>(Resource.Id.RefreshButton);
            RefreshBtn.Click += delegate {
                RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIconClicked);
                toastMessage.Show();
                dataItems.Clear();
                displayItems.Clear();
                GraphButton.Enabled = false;

                NameValueCollection values = new NameValueCollection();
                values.Add("Type", "Bank");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e) {
                ClientProgress.Progress += e.ProgressPercentage;
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<BankBalance>>(json);
                    adapter = new BankViewAdapter(this.Context, dataItems);//this

                    //create variables used to find the latest bank balance value
                    DateTime latestDate = new DateTime(1000, 1, 1);
                    string BankBalanceValue = "";
                    foreach (BankBalance item in dataItems) {
                        DateTime dateOfItem = item.GetDate();
                        int result = DateTime.Compare(dateOfItem, latestDate);
                        //if dateOfItem is later than latestDate
                        if (result > 0) {
                            latestDate = dateOfItem;
                            BankBalanceValue = item.GetBankBalance().ToString("#,#", CultureInfo.InvariantCulture);
                        }
                    }

                    NumItems.Text = dataItems.Count.ToString();
                    GraphButton.Enabled = true;

                    BankBalanceView.Text = " $ " + BankBalanceValue;
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    toastMessage.Cancel();
                    adapter.NotifyDataSetChanged();
                });
            };

            BalanceBtn.Click += delegate {
                if (clickNumBalance == 0) {
                    dataItems.Sort(delegate (BankBalance one, BankBalance two) {
                        return one.GetBankBalance().CompareTo(two.GetBankBalance());
                    });
                    clickNumBalance++;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumBalance = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            DateBtn.Click += delegate {
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (BankBalance one, BankBalance two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumBalance = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }
    }
}