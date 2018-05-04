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
    public class FinanceData {
        protected string FirstName;
        protected string LastName;
        protected int Age;
        protected string Gender;
        protected double Income;
        protected string Colour;

        //each time an object is created, this value increments by 1
        public static int NumberOfIncomeData;

        //constructor accepts a name string input and a income float input
        public FinanceData(string FirstNameInput, string LastNameInput, int AgeInput, string GenerInput, double IncomeInput) {
            this.FirstName = FirstNameInput;
            this.LastName = LastNameInput;
            this.Age = AgeInput;
            this.Gender = GenerInput;
            this.Income = IncomeInput;
            NumberOfIncomeData++;
        }

        //retrieve first name value
        public string GetFirstName() {
            return FirstName;
        }

        //retrieve last name value
        public string GetLastName() {
            return LastName;
        }

        //retrieve age value
        public int GetAge()  {
            return Age;
        }

        //retrieve gender value
        public string GetGender() {
            return Gender;
        }

        //retrieve income value as string
        public string GetIncome() {
            return Income.ToString();
        }

        //retrieve the number of income data available
        public int GetNumOfData() {
            return NumberOfIncomeData;
        }

        //if the income is above or equal to 500.00 return true
        public bool IsGreen() {
            if (Income >= 500.0) {
                return true;
            } else {
                return false;
            }
        }

        //if the income is less than 250.00 return true
        public bool IsRed() {
            if (Income < 250.0) {
                return true;
            } else {
                return false;
            }
        }
    }

    public class StaffData {

    }

    public class ResidentData {

    }
}