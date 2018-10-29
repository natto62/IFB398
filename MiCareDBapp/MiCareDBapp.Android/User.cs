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

namespace MiCareDBapp.Droid
{
    public class User
    {
        //User object
        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }

        //retrieveid value
        public int GetUserID()
        {
            return UserID;
        }

        //retrieve first name value
        public string GetFirstName()
        {
            return FName;
        }

        //retrieve last name value
        public string GetLastName()
        {
            return LName;
        }

        //retrieve last name value
        public string GetEmail()
        {
            return Email;
        }

    }
}