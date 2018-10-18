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
    [Activity(Label = "ItemPage", Theme = "@style/MainTheme")]
    public class HomeCarePackagePage : Android.Support.V4.App.Fragment
    {
        //int number;

        private ListView dataList;
        private List<HomeCarePackageData> dataItems;
        private List<HomeCarePackageData> displayItems;
        private List<HomeCarePackageData> searchItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumIncome = 0;
        private int clcikNumLevel = 0;

        private HomeCarePackageViewAdapter adapter;

        private WebClient client;
        private Uri url;
        private TextView NumItems;

        private Toast toastMessage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.HomeCarePackagePage, container, false);

            dataItems = new List<HomeCarePackageData>();
            displayItems = new List<HomeCarePackageData>();
            searchItems = new List<HomeCarePackageData>();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);
            adapter = new HomeCarePackageViewAdapter(this.Context, dataItems);//view or this ill fix later, note to self

            //Display the number of items at the bottom of the page
            NumItems = view.FindViewById<TextView>(Resource.Id.txtNumData);

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = view.FindViewById<Button>(Resource.Id.FirstNameTextHomeCare);
            Button LName = view.FindViewById<Button>(Resource.Id.LastNameTextHomeCare);
            Button PackageLevel = view.FindViewById<Button>(Resource.Id.PackageLevelText);
            Button IncomeList = view.FindViewById<Button>(Resource.Id.PackageIncomeText);

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
                HomeCarePackagesGraph info = new HomeCarePackagesGraph(dataItems);
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
                foreach (HomeCarePackageData item in dataItems) {
                    if (!searchItems.Contains(item)) {
                        searchItems.Add(item);
                    }
                }
                foreach (HomeCarePackageData item in searchItems) {
                    bool foundFName = String.Equals(searchName, item.GetResidentFirstName(), StringComparison.OrdinalIgnoreCase);//ignores upper or lower case
                    bool foundLName = String.Equals(searchName, item.GetResidentLastName(), StringComparison.OrdinalIgnoreCase);
                    bool insideName = item.GetResidentFirstName().Contains(searchName) || item.GetResidentLastName().Contains(searchName);
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
                foreach (HomeCarePackageData item in searchItems) {
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
                values.Add("Type", "Home");
                //call php 
                client.UploadValuesAsync(url, values);
            };

            client.UploadValuesCompleted += delegate (object sender, UploadValuesCompletedEventArgs e) {
                Activity.RunOnUiThread(() => {
                    string json = Encoding.UTF8.GetString(e.Result);
                    dataItems = JsonConvert.DeserializeObject<List<HomeCarePackageData>>(json);
                    adapter = new HomeCarePackageViewAdapter(this.Context, dataItems);//this
                    foreach (HomeCarePackageData item in dataItems) {
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
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return string.Compare(one.GetResidentFirstName(), two.GetResidentFirstName());
                    });
                    clickNumFName++;
                    clickNumLName = 0;
                    clickNumIncome = 0;
                    clcikNumLevel = 0;
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
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return string.Compare(one.GetResidentLastName(), two.GetResidentLastName());
                    });
                    clickNumLName++;
                    clickNumFName = 0;
                    clickNumIncome = 0;
                    clcikNumLevel = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumLName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on package levels
            PackageLevel.Click += delegate {
                if (clcikNumLevel == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return one.GetPackageLevel().CompareTo(two.GetPackageLevel());
                    });
                    clcikNumLevel++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumIncome = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clcikNumLevel = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on incomes
            IncomeList.Click += delegate {
                if (clickNumIncome == 0)
                {
                    dataItems.Sort(delegate (HomeCarePackageData one, HomeCarePackageData two) {
                        return one.GetPackageIncome().CompareTo(two.GetPackageIncome());
                    });
                    clickNumIncome++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clcikNumLevel = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumIncome = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //if an item in the list is clicked, then create a pop up window with more information on the item clicked
            dataList.ItemClick += DataList_ItemClick;

            return view;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int ID;
            searchItems.Clear();
            int position = e.Position;
            foreach (HomeCarePackageData item in displayItems) {
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
    

        //create a pop up window with more information
        void DataList_ItemClick(object sender, AdapterView.ItemClickEventArgs e) {
            //FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //MoreInfo info = new MoreInfo(dataItems[e.Position]);
            //info.Show(transaction, "dialog fragment");

            var transaction = ChildFragmentManager.BeginTransaction();
            MoreInfo info = new MoreInfo(dataItems[e.Position]);
            info.Show(transaction, "dialog fragment");
        }


        public void NotifyAdapter()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}