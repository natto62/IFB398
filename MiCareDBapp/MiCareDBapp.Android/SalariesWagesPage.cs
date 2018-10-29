using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace MiCareDBapp.Droid
{
    [Activity(Label = "SalariesWagesPage", Theme = "@style/MainTheme")]
    public class SalariesWagesPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<SalariesWagesData> dataItems;
        private List<SalariesWagesData> displayItems;

        private int clickNumDate = 0;
        private int clickNumActual = 0;
        private int clickNumBudget = 0;
        private int clickNumVariance = 0;

        private SalariesWagesViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {   //OnCreate takes a Bundle parameter, which is a dictionary for storing and passing state information 
            //and objects between activities If the bundle is not null, this indicates the activity is restarting 
            //and it should restore its state from the previous instance. "https:/docs.microsoft.com/en-us/xamarin/android/app-fundamentals/activity-lifecycle/"
            base.OnCreateView(inflater, container, savedInstanceState);
            //Once on create has finished, android will call OnStart which will start the activity

            //sets the layout of the main menu to the SalariesWagesPage.axml file which is located in Resources/layout/
            View view = inflater.Inflate(Resource.Layout.SalariesWagesPage, container, false);

            //setup lists for salaries and wages data
            dataItems = new List<SalariesWagesData>();
            displayItems = new List<SalariesWagesData>();

            //setup custom list adapter, more info found in SalariesWagesViewAdapter.cs
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new SalariesWagesViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextSalariesWages);
            Button ActualCostBtn = view.FindViewById<Button>(Resource.Id.ActualCostTextSalariesWages);
            Button BudgetBtn = view.FindViewById<Button>(Resource.Id.BudgetTextSalariesWages);
            Button VarianceBtn = view.FindViewById<Button>(Resource.Id.VarianceTextSalariesWages);

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
                SalariesWagesGraph info = new SalariesWagesGraph(displayItems);
                info.Show(transaction, "dialog fragment");
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
                values.Add("Type", "Salaries");
                //call php file and use UploadValuesAsync with the value of Type=Salaries so the php file knows to pull salaries and wages data
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<SalariesWagesData>>(json);//use json to create a list of data objects from the output of the php file
                    adapter = new SalariesWagesViewAdapter(this.Context, dataItems);//setup adapter
                    foreach (SalariesWagesData item in dataItems)
                    {
                        displayItems.Add(item);//display items holds all of the data objects for safe keeping, for when dataItems objects get removed
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
            //sort data via date
            DateBtn.Click += delegate {
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });

                    clickNumDate++;
                    clickNumActual = 0;
                    clickNumBudget = 0;
                    clickNumVariance = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };
            //sort data via actual cost
            ActualCostBtn.Click += delegate {
                if (clickNumActual == 0)
                {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return one.GetActualCost().CompareTo(two.GetActualCost());
                    });

                    clickNumActual++;
                    clickNumDate = 0;
                    clickNumBudget = 0;
                    clickNumVariance = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumActual = 0;
                }
                adapter.NotifyDataSetChanged();
            };
            //sort data via budget
            BudgetBtn.Click += delegate {
                if (clickNumBudget == 0) {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return one.GetBudget().CompareTo(two.GetBudget());
                    });
                    clickNumBudget++;
                    clickNumActual = 0;
                    clickNumDate = 0;
                    clickNumVariance = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumBudget = 0;
                }
                adapter.NotifyDataSetChanged();
            };
            //sort data via variance
            VarianceBtn.Click += delegate {
                if (clickNumVariance == 0) {
                    dataItems.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                        return one.GetVariance().CompareTo(two.GetVariance());
                    });
                    clickNumVariance++;
                    clickNumActual = 0;
                    clickNumDate = 0;
                    clickNumBudget = 0;
                    //reverse list if clicked a second time in a row
                } else {
                    dataItems.Reverse();
                    clickNumVariance = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }
        //the spinner for selecting a facility
        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;//get spinner position
            Console.WriteLine(position.ToString());
            foreach (SalariesWagesData item in displayItems)
            {
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