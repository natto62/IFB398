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

namespace MiCareApp.Droid
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
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumFinanceData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextBrokerage);
            Button HoursBtn = view.FindViewById<Button>(Resource.Id.HoursTextBrokerage);

            //setup Spinner
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

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

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            foreach (BrokerageHoursData item in displayItems)
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