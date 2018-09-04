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
    public class User
    {

        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }
        //protected DateTime Date;

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