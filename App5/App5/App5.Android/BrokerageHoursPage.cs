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
    [Activity(Label = "BrokerageHoursPage", Theme = "@style/MainTheme")]
    public class BrokerageHoursPage : Activity
    {

        private ListView dataList;
        private List<BrokerageHoursData> dataItems;
        private List<BrokerageHoursData> displayItems;

        private int clickNumDate = 0;
        private int clickNumHours = 0;

        private BrokerageHoursViewAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BrokerageHoursPage);


            dataItems = new List<BrokerageHoursData>();
            displayItems = new List<BrokerageHoursData>();

            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 6, 1), 10, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 6, 1), 17, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 6, 1), 25, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 7, 1), 10, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 7, 1), 11, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 7, 1), 14, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 8, 1), 25, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 8, 1), 21, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 8, 1), 13, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 9, 1), 20, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 9, 1), 12, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 9, 1), 16, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 10, 1), 12, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 10, 1), 15, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 10, 1), 17, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 11, 1), 27, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 11, 1), 22, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 11, 1), 10, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 12, 1), 30, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 12, 1), 13, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2018, 12, 1), 11, 3));
            dataItems.Add(new BrokerageHoursData(new DateTime(2019, 1, 1), 20, 1));
            dataItems.Add(new BrokerageHoursData(new DateTime(2019, 1, 1), 20, 2));
            dataItems.Add(new BrokerageHoursData(new DateTime(2019, 1, 1), 10, 3));

            foreach (BrokerageHoursData item in dataItems)
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

            adapter = new BrokerageHoursViewAdapter(this, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = FindViewById<Button>(Resource.Id.DateTextBrokerage);
            Button HoursBtn = FindViewById<Button>(Resource.Id.HoursTextBrokerage);

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



            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            Console.WriteLine(position.ToString());
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