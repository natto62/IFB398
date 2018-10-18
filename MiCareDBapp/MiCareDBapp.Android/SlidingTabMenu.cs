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
using Java.Lang;

namespace MiCareDBapp.Droid
{
    class SlidingTabMenu : FragmentPagerAdapter
    {
        private string[] Names;
        private List<Android.Support.V4.App.Fragment> fragments;
        private bool type;

        //constructor 
        public SlidingTabMenu(Android.Support.V4.App.FragmentManager fm, string[] names, bool type) : base(fm) {
            Names = names;
            this.type = type;
            fragments = new List<Android.Support.V4.App.Fragment>();
            if (type) {
                fragments.Add(new ACFIPage());
                fragments.Add(new AgencyUsagePage());
                fragments.Add(new BankPage());
                fragments.Add(new BrokerageHoursPage());
                fragments.Add(new HomeCarePackagePage());
                fragments.Add(new IncomePage());
                fragments.Add(new SalariesWagesPage());
            } else {
                fragments.Add(new OccupancyPage());
                fragments.Add(new StaffPage());
            }

        }

        //number of tabs
        public override int Count { get { if (type) { return 7; } else { return 2; } } }

        //get fragment via tab position
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        //display tab titles
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) {
            return new Java.Lang.String(Names[position]);
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PositionNone;
        }

        //get fragment via tab position
        public Android.Support.V4.App.Fragment GetFragment(int position)
        {
            return fragments[position];
        }

        public List<Android.Support.V4.App.Fragment> GetFragments() {
            return fragments;
        }
    }
}