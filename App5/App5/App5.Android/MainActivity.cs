using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using App5.Droid;
using System.Collections.Generic;

namespace MiCareApp.Droid
{
    [Activity(Label = "App5", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        //int number;

        private ListView dataList;

        private List<FinanceData> dataItems;

        //private List<string> Names;
        
        //private List<string> Incomes;

        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MainTitle);
            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());

            dataItems = new List<FinanceData>();
            //Names = new List<string>();
            //Incomes = new List<string>();


            dataItems.Add(new FinanceData("Bill", "Person", 64, "male", 843.65));
            dataItems.Add(new FinanceData("Sarah", "Thingy", 72, "female", 412.80));
            dataItems.Add(new FinanceData("Brian", "Brown", 101, "male", 124.30));

            //foreach (FinanceData data in dataItems) {
            //    Names.Add(data.GetFirstName());
            //    Incomes.Add(data.GetIncome());
            //}

            dataList = FindViewById<ListView>(Resource.Id.DataList);

            MyListViewAdapter adapter = new MyListViewAdapter(this, dataItems);

            dataList.Adapter = adapter;
            dataList.ItemClick += DataList_ItemClick;

        }

        void DataList_ItemClick(object sender, AdapterView.ItemClickEventArgs e) {
            //go to an information page with more info
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            MoreInfo info = new MoreInfo();
            info.Show(transaction, "dialog fragment");
        }
    }
}

