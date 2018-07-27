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

namespace MiCareApp.Droid
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.AgencyUsagePage, container, false);

            dataItems = new List<AgencyUsageData>();
            displayItems = new List<AgencyUsageData>();

            dataItems.Add(new AgencyUsageData(new DateTime(2018, 6, 1), 2876, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 6, 1), 2107, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 6, 1), 4579, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 7, 1), 4948, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 7, 1), 3470, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 7, 1), 2482, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 8, 1), 3710, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 8, 1), 2843, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 8, 1), 2406, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 9, 1), 3507, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 9, 1), 2970, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 9, 1), 2167, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 10, 1), 3383, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 10, 1), 2238, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 10, 1), 3626, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 11, 1), 3906, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 11, 1), 3084, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 11, 1), 3758, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 12, 1), 3645, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 12, 1), 3985, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2018, 12, 1), 4765, 3));
            dataItems.Add(new AgencyUsageData(new DateTime(2019, 1, 1), 3237, 1));
            dataItems.Add(new AgencyUsageData(new DateTime(2019, 1, 1), 3891, 2));
            dataItems.Add(new AgencyUsageData(new DateTime(2019, 1, 1), 2484, 3));

            foreach (AgencyUsageData item in dataItems) {
                displayItems.Add(item);
            }

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);

            adapter = new AgencyUsageViewAdapter(view.Context, dataItems);

            dataList.Adapter = adapter;

            //setup Spinner
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(view.Context, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //Display the number of items at the bottom of the page
            TextView NumItems = view.FindViewById<TextView>(Resource.Id.txtNumFinanceData);
            NumItems.Text = dataItems.Count.ToString();

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = view.FindViewById<Button>(Resource.Id.DateTextAgency);
            Button AmountBtn = view.FindViewById<Button>(Resource.Id.AmountTextAgency);

            DateBtn.Click += delegate {
                if (clickNumDate == 0)
                {
                    dataItems.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumAmount = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumDate = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            AmountBtn.Click += delegate {
                if (clickNumAmount == 0)
                {
                    dataItems.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                        return one.GetAgencyUsageAmount().CompareTo(two.GetAgencyUsageAmount());
                    });
                    clickNumAmount++;
                    clickNumDate = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumAmount = 0;
                }
                adapter.NotifyDataSetChanged();
            };


            Button backBtn = view.FindViewById<Button>(Resource.Id.BackButton);
            backBtn.Click += delegate {
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                StartActivity(new Intent(Activity, typeof(FinanceMenu)));
            };
            return view;
        }

        void Spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e) {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            Console.WriteLine(position.ToString());
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