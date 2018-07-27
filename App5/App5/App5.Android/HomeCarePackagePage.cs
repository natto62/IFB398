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
    public class HomeCarePackagePage : Android.Support.V4.App.Fragment
    {
        //int number;

        private ListView dataList;

        private List<HomeCarePackageData> dataItems;

        private int clickNumFName = 0;
        private int clickNumLName = 0;
        private int clickNumIncome = 0;
        private int clcikNumLevel = 0;

        //private List<string> Names;

        //private List<string> Incomes;
        private HomeCarePackageViewAdapter adapter;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.HomeCarePackagePage, container, false);
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());

            dataItems = new List<HomeCarePackageData>();

            //Names = new List<string>();
            //Incomes = new List<string>();

            //foreach (FinanceData data in dataItems) {
            //    Names.Add(data.GetFirstName());
            //    Incomes.Add(data.GetIncome());
            //}

            //add items
            dataItems.Add(new HomeCarePackageData(1,"Mia", "Carter", 1, 106));
            dataItems.Add(new HomeCarePackageData(2,"Michael", "Howard", 1, 176));
            dataItems.Add(new HomeCarePackageData(3,"Logan", "Watson", 2, 100));
            dataItems.Add(new HomeCarePackageData(4, "Harper", "Simmons", 3, 140));
            dataItems.Add(new HomeCarePackageData(5, "Olivia", "Ross", 2, 204));
            dataItems.Add(new HomeCarePackageData(6, "James", "Smith", 1, 148));
            dataItems.Add(new HomeCarePackageData(7, "Sally", "Johnson", 4, 188));
            dataItems.Add(new HomeCarePackageData(8, "Jim", "Williams", 1, 266));
            dataItems.Add(new HomeCarePackageData(9, "Sophia", "Jones", 1, 278));
            dataItems.Add(new HomeCarePackageData(10, "Jackson", "Brown", 2, 127));
            dataItems.Add(new HomeCarePackageData(11, "Lucas", "Davis", 1, 223));
            dataItems.Add(new HomeCarePackageData(12, "Liam", "Miller", 4, 157));
            dataItems.Add(new HomeCarePackageData(13, "Noah", "Scot", 3, 194));
            dataItems.Add(new HomeCarePackageData(14, "Riley", "Hill", 3, 211));


            //Display the number of items at the bottom of the page
            TextView NumItems = view.FindViewById<TextView>(Resource.Id.txtNumFinanceData);
            NumItems.Text = dataItems.Count.ToString();

            //setup adapter
            dataList = view.FindViewById<ListView>(Resource.Id.DataList);

            adapter = new HomeCarePackageViewAdapter(view.Context, dataItems);

            dataList.Adapter = adapter;

            //setup buttons at the top of the page which are used to sort the list based on the button pushed
            Button FName = view.FindViewById<Button>(Resource.Id.FirstNameTextHomeCare);
            Button LName = view.FindViewById<Button>(Resource.Id.LastNameTextHomeCare);
            Button PackageLevel = view.FindViewById<Button>(Resource.Id.PackageLevelText);
            Button IncomeList = view.FindViewById<Button>(Resource.Id.PackageIncomeText);

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

            Button backBtn = view.FindViewById<Button>(Resource.Id.BackButton);

            backBtn.Click += delegate {
                backBtn.SetBackgroundResource(Resource.Drawable.BackButtonIconClicked);
                StartActivity(new Intent(Activity, typeof(FinanceMenu)));
            };

            return view;
        }

        //create a pop up window with more information
        void DataList_ItemClick(object sender, AdapterView.ItemClickEventArgs e) {
          //  FragmentTransaction transaction = FragmentManager.BeginTransaction();
          //  MoreInfo info = new MoreInfo(dataItems[e.Position]);
          //  info.Show(transaction, "dialog fragment");
        }

    }
}