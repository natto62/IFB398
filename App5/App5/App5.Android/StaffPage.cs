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

namespace MiCareApp.Droid
{
    [Activity(Label = "StaffPage", Theme = "@style/MainTheme")]
    public class StaffPage : Activity
    {

        private ListView dataList;
        private List<StaffData> dataItems;
        private List<StaffData> displayItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumAnnual = 0;
        private int clickNumLongService = 0;
        private int clickNumService = 0;

        private StaffViewAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.StaffPage);


            dataItems = new List<StaffData>();
            displayItems = new List<StaffData>();

            dataItems.Add(new StaffData(2, 4, "Sallv", "Stupid", 29, 26, 5));
            dataItems.Add(new StaffData(3, 34, "Ron", "Crows", 2, 18, 4));
            dataItems.Add(new StaffData(3, 35, "Mia", "Poultrv", 20, 18, 16));
            dataItems.Add(new StaffData(1, 55, "Jim", "Smith", 2, 48, 13));
            dataItems.Add(new StaffData(2, 56, "Sandra", "Dingus", 23, 40, 12));
            dataItems.Add(new StaffData(1, 78, "Sarah", "Simmons", 22, 22, 15));
            dataItems.Add(new StaffData(1, 125, "John", "Johnson", 7, 10, 12));
            dataItems.Add(new StaffData(3, 131, "Bob", "Pigs", 7, 42, 10));
            dataItems.Add(new StaffData(2, 341, "James", "Apple", 10, 32, 14));

            foreach (StaffData item in dataItems)
            {
                displayItems.Add(item);
            }


            //setup Spinner
            Spinner spinner = FindViewById<Spinner>(Resource.Id.FacilitySpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var SpinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.FacilityArray, Android.Resource.Layout.SimpleSpinnerItem);
            SpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = SpinnerAdapter;

            //Display the number of items at the bottom of the page
            TextView NumItems = FindViewById<TextView>(Resource.Id.txtNumFinanceData);
            NumItems.Text = dataItems.Count.ToString();

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            adapter = new StaffViewAdapter(this, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = FindViewById<Button>(Resource.Id.FirstNameTextStaff);
            Button LName = FindViewById<Button>(Resource.Id.LastNameTextStaff);
            Button AnnualBtn = FindViewById<Button>(Resource.Id.AnnualLeaveTextStaff);
            Button LongServiceBtn = FindViewById<Button>(Resource.Id.LongServiceLeaveTextStaff);
            Button ServiceBtn = FindViewById<Button>(Resource.Id.ServiceLeaveTextStaff);

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
                    clickNumService = 0;
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
                    clickNumService = 0;
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
                    clickNumService = 0;
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
                    clickNumService = 0;
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
            ServiceBtn.Click += delegate {
                if (clickNumService == 0)
                {
                    dataItems.Sort(delegate (StaffData one, StaffData two) {
                        return one.GetServiceLeaveAcrewed().CompareTo(two.GetServiceLeaveAcrewed());
                    });
                    clickNumService++;
                    clickNumLName = 0;
                    clickNumFName = 0;
                    clickNumAnnual = 0;
                    clickNumLongService = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumService = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(OccupancyMenu)); };
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            Console.WriteLine(position.ToString());
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
    }
}