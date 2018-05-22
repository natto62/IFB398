using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using App5.Droid;
using Android.Widget;

namespace MiCareApp.Droid
{
    [Activity(Label = "ItemPage", Theme = "@style/MainTheme")]
    public class ItemPage : Activity
    {
        //int number;

        private ListView dataList;

        private List<FinanceData> dataItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumIncome = 0;

        //private List<string> Names;

        //private List<string> Incomes;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainTitle);
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());

            dataItems = new List<FinanceData>();
            //Names = new List<string>();
            //Incomes = new List<string>();

            //foreach (FinanceData data in dataItems) {
            //    Names.Add(data.GetFirstName());
            //    Incomes.Add(data.GetIncome());
            //}

            //add items
            dataItems.Add(new FinanceData("Bill", "Person", 64, "male", 843.65));
            dataItems.Add(new FinanceData("Sarah", "Thingy", 72, "female", 412.80));
            dataItems.Add(new FinanceData("Brian", "Brown", 101, "male", 124.30));
            dataItems.Add(new FinanceData("Moe", "Whato", 57, "male", 354.16));
            dataItems.Add(new FinanceData("Lesli", "Crump", 68, "female", 784.98));
            dataItems.Add(new FinanceData("Olivia", "Something", 81, "female", 87.83));

            //Display the number of items at the bottom of the page
            TextView NumItems = FindViewById<TextView>(Resource.Id.txtNumFinanceData);
            NumItems.Text = dataItems.Count.ToString();

            //setup adapter
            dataList = FindViewById<ListView>(Resource.Id.DataList);

            MyListViewAdapter adapter = new MyListViewAdapter(this, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = FindViewById<Button>(Resource.Id.FirstNameText);
            Button LName = FindViewById<Button>(Resource.Id.LastNameText);
            Button IncomeList = FindViewById<Button>(Resource.Id.IncomeText);

            //setup button for sorting the list based on first names
            FName.Click += delegate {
                if (clickNumFName == 0)
                {
                    dataItems.Sort(delegate (FinanceData one, FinanceData two) {
                        return string.Compare(one.GetFirstName(), two.GetFirstName());
                    });
                    clickNumFName++;
                    clickNumLName = 0;
                    clickNumIncome = 0;
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
                    dataItems.Sort(delegate (FinanceData one, FinanceData two) {
                        return string.Compare(one.GetLastName(), two.GetLastName());
                    });
                    clickNumLName++;
                    clickNumFName = 0;
                    clickNumIncome = 0;
                    //reverse list if clicked a second time in a row
                }
                else
                {
                    dataItems.Reverse();
                    clickNumLName = 0;
                }
                adapter.NotifyDataSetChanged();
            };

            //setup button for sorting the list based on incomes
            IncomeList.Click += delegate {
                if (clickNumIncome == 0)
                {
                    dataItems.Sort(delegate (FinanceData one, FinanceData two) {
                        return one.GetIncomeAsDouble().CompareTo(two.GetIncomeAsDouble());
                    });
                    clickNumIncome++;
                    clickNumLName = 0;
                    clickNumFName = 0;
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

            Button backBtn = FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate { StartActivity(typeof(IntroPage)); };

        }

        //create a pop up window with more information
        void DataList_ItemClick(object sender, AdapterView.ItemClickEventArgs e) {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            MoreInfo info = new MoreInfo(dataItems[e.Position]);
            info.Show(transaction, "dialog fragment");
        }
    }
}