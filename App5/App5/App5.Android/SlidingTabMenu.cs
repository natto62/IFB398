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

namespace MiCareApp.Droid
{
    class SlidingTabMenu : FragmentPagerAdapter
    {
        private string[] Names;
        private List<Android.Support.V4.App.Fragment> fragments;

        //constructor 
        public SlidingTabMenu(Android.Support.V4.App.FragmentManager fm, string[] names) : base(fm) {
            Names = names;
            fragments = new List<Android.Support.V4.App.Fragment>();
            fragments.Add(new AgencyUsagePage());
            fragments.Add(new BrokerageHoursPage());
            fragments.Add(new HomeCarePackagePage());
            fragments.Add(new SalariesWagesPage());
        }

        //number of tabs
        public override int Count { get { return 4; } }

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

    }
}