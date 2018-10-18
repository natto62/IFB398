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
    [Activity(Label = "StaffPage", Theme = "@style/MainTheme")]
    public class StaffPage : Android.Support.V4.App.Fragment
    {

        private ListView dataList;
        private List<StaffData> dataItems;
        private List<StaffData> displayItems;
        private List<StaffData> searchItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumAnnual = 0;
        private int clickNumLongService = 0;
        private int clickNumSick = 0;

        private StaffViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.StaffPage, container, false);

            dataItems = new List<StaffData>();
            displayItems = new List<StaffData>();
            searchItems = new List<StaffData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new StaffViewAdapter(this.Context, dataItems);

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = view.FindViewById<Button>(Resource.Id.FirstNameTextStaff);
            Button LName = view.FindViewById<Button>(Resource.Id.LastNameTextStaff);
            Button AnnualBtn = view.FindViewById<Button>(Resource.Id.AnnualLeaveTextStaff);
            Button LongServiceBtn = view.FindViewById<Button>(Resource.Id.LongServiceLeaveTextStaff);
            Button SickBtn = view.FindViewById<Button>(Resource.Id.SickLeaveTextStaff);

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
                StaffGraph info = new StaffGraph(displayItems);
                info.Show(transaction, "dialog fragment");
            };

            //setup search bar
            SearchView SearchItems = view.FindViewById<SearchView>(Resource.Id.searchData);
            //an X apears next the search upon a submitted query, this X closes the current search
            int closeBtnID = SearchItems.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
            var SearchCloseBtn = view.FindViewById<ImageView>(closeBtnID);
            SearchItems.SetIconifiedByDefault(false);//shows hint
            SearchItems.Enabled = false;
            SearchItems.QueryTextSubmit += delegate {
                string searchName = SearchItems.Query;
                foreach (StaffData item in dataItems) {
                    if (!searchItems.Contains(item)) {
                        searchItems.Add(item);
                    }
                }
                foreach (StaffData item in searchItems) {
                    bool foundFName = String.Equals(searchName, item.GetStaffFirstName(), StringComparison.OrdinalIgnoreCase);//ignores upper or lower case
                    bool foundLName = String.Equals(searchName, item.GetStaffLastName(), StringComparison.OrdinalIgnoreCase);
                    bool insideName = item.GetStaffFirstName().Contains(searchName) || item.GetStaffLastName().Contains(searchName);
                    if (dataItems.Contains(item) && !foundFName && !foundLName && !insideName) {
                        dataItems.Remove(item);
                    } else if (!dataItems.Contains(item) && (foundFName || foundLName || insideName)) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                adapter.NotifyDataSetChanged();
            };
            SearchCloseBtn.Click += delegate {
                foreach (StaffData item in searchItems) {
                    if (!dataItems.Contains(item)) {
                        dataItems.Add(item);
                    }
                }
                SearchItems.ClearFocus();
                SearchItems.SetQuery(String.Empty, false);
                adapter.NotifyDataSetChanged();
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
                values.Add("Type", "Staff");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<StaffData>>(json);
                    adapter = new StaffViewAdapter(this.Context, dataItems);
                    foreach (StaffData item in dataItems) {
                        displayItems.Add(item);
                    }
                    NumItems.Text = dataItems.Count.ToString();
                    dataList.Adapter = adapter;
                    RefreshBtn.SetBackgroundResource(Resource.Drawable.RefreshButtonIcon);
                    spinner.Clickable = true;
                    GraphButton.Enabled = true;
                    SearchItems.Enabled = true;
                    toastMessage.Cancel();
                    searchItems.Clear();
                    adapter.NotifyDataSetChanged();
                });
            };

            //setup button for sorting the list based on first names
            FName.Click += delegate {
                if (clickNumFName == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return string.Compare(one.GetStaffFirstName(), two.GetStaffFirstName());
                    });
                    clickNumFName++;
                    clickNumLName = 0;
                    clickNumAnnual = 0;
                    clickNumLongService = 0;
                    clickNumSick = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumFName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on last names
            LName.Click += delegate {
                if (clickNumLName == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return string.Compare(one.GetStaffLastName(), two.GetStaffLastName());
                    });
                    clickNumLName++;
                    clickNumFName = 0;
                    clickNumAnnual = 0;
                    clickNumLongService = 0;
                    clickNumSick = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumLName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on annual leave
            AnnualBtn.Click += delegate {
                if (clickNumAnnual == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return one.GetAnnualLeaveAcrewed().CompareTo(two.GetAnnualLeaveAcrewed());
                    });
                    clickNumAnnual++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumLongService = 0;
                    clickNumSick = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumAnnual = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on long service leave
            LongServiceBtn.Click += delegate {
                if (clickNumLongService == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return one.GetLongServiceLeaveAcrewed().CompareTo(two.GetLongServiceLeaveAcrewed());
                    });
                    clickNumLongService++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumAnnual = 0;
                    clickNumSick = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumLongService = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on service leave
            SickBtn.Click += delegate {
                if (clickNumSick == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return one.GetSickLeaveAcrewed().CompareTo(two.GetSickLeaveAcrewed());
                    });
                    clickNumSick++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumAnnual = 0;
                    clickNumLongService = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumSick = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            return view;
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            searchItems.Clear();
            int position = e.Position;
            foreach (StaffData item in displayItems)
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

        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}