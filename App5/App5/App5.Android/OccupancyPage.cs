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
    [Activity(Label = "OccupancyPage", Theme = "@style/MainTheme")]
    public class OccupancyPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<OccupancyData> dataItems;
        private List<OccupancyData> displayItems;

        private int clickNumDate = 0;
        private int clickNumOccupancy = 0;
        private int clickNumConcessional = 0;
        private int clickNumCareType = 0;

        private OccupancyViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

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
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumFinanceData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextOccupancy);
            Button OccupancyBtn = view.FindViewById<Button>(Resource.Id.OccupancyTextOccupancy);
            Button CareTypeBtn = view.FindViewById<Button>(Resource.Id.CareTypeTextOccupancy);
            Button ConcessionalBtn = view.FindViewById<Button>(Resource.Id.ConcessionalTextOccupancy);

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
                    clickNumOccupancy = 0;
                    clickNumConcessional = 0;
                    clickNumCareType = 0;
                //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            OccupancyBtn.Click += delegate {
                if (clickNumOccupancy == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetOccupancy().CompareTo(two.GetOccupancy());
                    });
                    clickNumOccupancy++;
                    clickNumDate = 0;
                    clickNumConcessional = 0;
                    clickNumCareType = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumOccupancy = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            CareTypeBtn.Click += delegate {
                if (clickNumCareType == 0)
                {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return string.Compare(one.GetCareType(), two.GetCareType());
                    });
                    clickNumCareType++;
                    clickNumOccupancy = 0;
                    clickNumDate = 0;
                    clickNumConcessional = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumCareType = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            ConcessionalBtn.Click += delegate {
                if (clickNumConcessional == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return one.GetConcessional().CompareTo(two.GetConcessional());
                    });
                    clickNumConcessional++;
                    clickNumOccupancy = 0;
                    clickNumDate = 0;
                    clickNumCareType = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumConcessional = 0;
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
            foreach (OccupancyData item in displayItems)
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