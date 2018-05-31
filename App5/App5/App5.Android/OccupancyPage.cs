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
    [Activity(Label = "OccupancyPage", Theme = "@style/MainTheme")]
    public class OccupancyPage : Activity
    {

        private ListView dataList;
        private List<OccupancyData> dataItems;
        private List<OccupancyData> displayItems;

        private int clickNumDate = 0;
        private int clickNumOccupancy = 0;
        private int clickNumConcessional = 0;
        private int clickNumVacancy = 0;

        private OccupancyViewAdapter adapter;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.OccupancyPage);


            dataItems = new List<OccupancyData>();
            displayItems = new List<OccupancyData>();

            dataItems.Add(new OccupancyData(new DateTime(2018, 6, 1), 1, 1, 79, 29));
            dataItems.Add(new OccupancyData(new DateTime(2018, 6, 1), 1, 2, 101, 51));
            dataItems.Add(new OccupancyData(new DateTime(2018, 6, 1), 1, 3, 135, 85));
            dataItems.Add(new OccupancyData(new DateTime(2018, 7, 1), 2, 1, 260, 210));
            dataItems.Add(new OccupancyData(new DateTime(2018, 7, 1), 2, 2, 193, 143));
            dataItems.Add(new OccupancyData(new DateTime(2018, 7, 1), 2, 3, 209, 159));
            dataItems.Add(new OccupancyData(new DateTime(2018, 8, 1), 3, 1, 230, 180));
            dataItems.Add(new OccupancyData(new DateTime(2018, 8, 1), 3, 2, 118, 68));
            dataItems.Add(new OccupancyData(new DateTime(2018, 8, 1), 3, 3, 79, 29));
            dataItems.Add(new OccupancyData(new DateTime(2018, 9, 1), 1, 1, 195, 145));
            dataItems.Add(new OccupancyData(new DateTime(2018, 9, 1), 1, 2, 248, 198));
            dataItems.Add(new OccupancyData(new DateTime(2018, 9, 1), 1, 3, 124, 74));
            dataItems.Add(new OccupancyData(new DateTime(2018, 10, 1), 2, 1, 271, 221));
            dataItems.Add(new OccupancyData(new DateTime(2018, 10, 1), 2, 2, 179, 129));
            dataItems.Add(new OccupancyData(new DateTime(2018, 10, 1), 2, 3, 174, 124));
            dataItems.Add(new OccupancyData(new DateTime(2018, 11, 1), 3, 1, 52, 2));
            dataItems.Add(new OccupancyData(new DateTime(2018, 11, 1), 3, 2, 87, 37));
            dataItems.Add(new OccupancyData(new DateTime(2018, 11, 1), 3, 3, 195, 145));

            foreach (OccupancyData item in dataItems)
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

            adapter = new OccupancyViewAdapter(this, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button DateBtn = FindViewById<Button>(Resource.Id.DateTextOccupancy);
            Button OccupancyBtn = FindViewById<Button>(Resource.Id.OccupancyTextOccupancy);
            Button ConcessionalBtn = FindViewById<Button>(Resource.Id.ConcessionalTextOccupancy);

            DateBtn.Click += delegate {
                if (clickNumDate == 0) {
                    dataItems.Sort(delegate (OccupancyData one, OccupancyData two) {
                        return DateTime.Compare(one.GetDate(), two.GetDate());
                    });
                    clickNumDate++;
                    clickNumOccupancy = 0;
                    clickNumConcessional = 0;
                    clickNumVacancy = 0;
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
                    clickNumVacancy = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumOccupancy = 0;
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
                    clickNumVacancy = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumConcessional = 0;
                }
                adapter.NotifyDataSetChanged();
            };


            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate {
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                StartActivity(typeof(OccupancyMenu));
            };
        }

        void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int ID;
            int position = e.Position;
            Console.WriteLine(position.ToString());
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