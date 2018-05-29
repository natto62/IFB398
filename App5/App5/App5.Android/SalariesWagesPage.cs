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
    [Activity(Label = "SalariesWagesPage", Theme = "@style/MainTheme")]
    public class SalariesWagesPage : Activity
    {

        private ListView dataList;
        private List<SalariesWagesData> dataItems;
        private List<SalariesWagesData> displayItems;

        private int clickNumDate = 0;
        private int clickNumRostered = 0;
        private int clickNumBudget = 0;

        private SalariesWagesViewAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SalariesWagesPage);


            dataItems = new List<SalariesWagesData>();
            displayItems = new List<SalariesWagesData>();

            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 1), 1, 2731, 4769));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 1), 2, 4277, 3395));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 1), 3, 4643, 4604));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 2), 1, 2733, 3712));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 2), 2, 3497, 3686));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 2), 3, 2634, 2662));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 3), 1, 3269, 4858));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 3), 2, 3316, 4766));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 3), 3, 3890, 4685));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 4), 1, 3768, 4171));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 4), 2, 3664, 3038));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 4), 3, 3316, 2581));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 5), 1, 3956, 4925));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 5), 2, 2052, 3677));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 5), 3, 2816, 3614));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 6), 1, 4224, 4316));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 6), 2, 3423, 3358));
            dataItems.Add(new SalariesWagesData(new DateTime(2018, 6, 6), 3, 2880, 4570));

            foreach (SalariesWagesData item in dataItems)
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

            adapter = new SalariesWagesViewAdapter(this, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = FindViewById<Button>(Resource.Id.DateTextSalariesWages);
            Button RosteredCostBtn = FindViewById<Button>(Resource.Id.RosteredCostTextSalariesWages);
            Button BudgetBtn = FindViewById<Button>(Resource.Id.BudgetTextSalariesWages);

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


            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(FinanceMenu)); };
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