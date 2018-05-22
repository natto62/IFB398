using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MiCareApp.Droid
{
    class SignUp : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.SignUp, container, false);

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {

            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.SignInUpAnimation;
        }
    }
}