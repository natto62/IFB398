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
    public class User {
        protected string FirstName;
        protected string LastName;
        protected string Email;
        protected string Password;
        protected DateTime Date;

        //each time an object is created, this value increments by 1
        public static int NumberOfUserData;

        //constructor accepts a name string input and a income float input
        public User(string FirstNameInput, string LastNameInput, string EmailInput, string PasswordInput, DateTime DateInput) {
            this.FirstName = FirstNameInput;
            this.LastName = LastNameInput;
            this.Email = EmailInput;
            this.Password = PasswordInput;
            this.Date = DateInput;
            NumberOfUserData++;
        }

        //retrieve first name value
        public string GetFirstName() {
            return FirstName;
        }

        //retrieve last name value
        public string GetLastName() {
            return LastName;
        }

        //retrieve last name value
        public string GetEmail() {
            return Email;
        }

        //retrieve password value
        public string GetPassword() {
            return Password;
        }



    }
}