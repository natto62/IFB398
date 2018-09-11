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
    [Activity(Label = "SalariesWagesPage", Theme = "@style/MainTheme")]
    public class SalariesWagesPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<SalariesWagesData> dataItems;
        private List<SalariesWagesData> displayItems;

        private int clickNumDate = 0;
        private int clickNumRostered = 0;
        private int clickNumBudget = 0;

        private SalariesWagesViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.SalariesWagesPage, container, false);

            dataItems = new List<SalariesWagesData>();
            displayItems = new List<SalariesWagesData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new SalariesWagesViewAdapter(view.Context, dataItems);

            //Display the number of items at the bottom of the page
            TextView NumItems = view.FindViewById<TextView>(Resource.Id.txtNumFinanceData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextSalariesWages);
            Button RosteredCostBtn = view.FindViewById<Button>(Resource.Id.RosteredCostTextSalariesWages);
            Button BudgetBtn = view.FindViewById<Button>(Resource.Id.BudgetTextSalariesWages);

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
                values.Add("Type", "Salaries");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<SalariesWagesData>>(json);
                    adapter = new SalariesWagesViewAdapter(this.Context, dataItems);//this
                    foreach (SalariesWagesData item in dataItems)
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
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumRostered = 0;
                    clickNumBudget = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            RosteredCostBtn.Click += delegate {
                if (clickNumRostered == 0)
                {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return one.GetRosteredCost().CompareTo(two.GetRosteredCost());
                    });
                    clickNumRostered++;
                    clickNumDate = 0;
                    clickNumBudget = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumRostered = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            BudgetBtn.Click += delegate {
                if (clickNumBudget == 0)
                {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return one.GetBudget().CompareTo(two.GetBudget());
                    });
                    clickNumBudget++;
                    clickNumRostered = 0;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumBudget = 0;
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
            Console.WriteLine(position.ToString());
            foreach (SalariesWagesData item in displayItems)
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