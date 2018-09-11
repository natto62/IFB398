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

        //retrieve age value as string
        public string GetAge()  {
            return Age.ToString();
        }

        //retrieve gender value
        public string GetGender() {
            return Gender;
        }

        //retrieve income value as string
        public string GetIncome() {
            return Income.ToString();
        }

        //retrieve income value as double
        public double GetIncomeAsDouble() {
            return Income;
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

    public class ACFIFunding {

        public int facilityID { get; set; }
        public int residentID { get; set; }
        public string acfiScore { get; set; }
        public int income { get; set; }

        public int GetResidentID() {
            return residentID;
        }

        public string GetACFIScore() {
            return acfiScore;
        }

        public int GetIncome() {
            return income;
        }

        public int GetFacilityID() {
            return facilityID;
        }

    }

    public class BankBalance {

        public DateTime date { get; set; }
        public int balance { get; set; }
        public int facilityID { get; set; }
        protected bool show = true;


        public double GetBankBalance() {
            return balance;
        }

        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID()
        {
            return facilityID;
        }

        public void UpdateBankBalance(DateTime Date, int Balance) {
            this.date = Date;
            this.balance = Balance;
        }

        public void Show(bool value) {
            show = value;
        }

        public bool GetShow() {
            return show;
        }


    }

    public class AgencyUsageData {

        public DateTime Date { get; set; }
       // protected int InvoiceID;
        public double Amount { get; set; }
        public int FacilityID { get; set; }
        protected bool show = true;

        //public AgencyUsageData(DateTime Date, double Amount, int FacilityID) {
        //    this.Date = Date;
        //    //this.InvoiceID = InvoiceID;
        //    this.Amount = Amount;
        //    this.FacilityID = FacilityID;
        //}

        public DateTime GetDate() {
            return Date;
        }

        //public int GetInvoiceID() {
        //    return InvoiceID;
       // }

        public double GetAgencyUsageAmount() {
            return Amount;
        }

        public int GetFacilityID() {
            return FacilityID;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }
    }

    public class BrokerageHoursData {

        public DateTime date { get; set; }
        // protected int InvoiceID;
        public int hours { get; set; }
        public int facilityID { get; set; }
        protected bool show = true;

        public DateTime GetDate() {
            return date;
        }

        //public int GetBrokerageID() {
        //    return BrokerageID;
        //}

        public int GetBrokerageHours() {
            return hours;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }
    }

    public class HomeCarePackageData {

        public int facilityID { get; set; }
        public int residentID { get; set; }
        public string residentFirstName { get; set; }
        public string residentLastName { get; set; }
        public int packageLevel { get; set; }
        public int packageIncome { get; set; }
        protected bool show = true;



        public int GetResidentID() {
            return residentID;
        }

        public int GetFacilityID()
        {
            return facilityID;
        }

        public string GetResidentFirstName() {
            return residentFirstName;
        }

        public string GetResidentLastName() {
            return residentLastName;
        }

        public int GetPackageLevel() {
            return packageLevel;
        }

        public int GetPackageIncome() {
            return packageIncome;
        }

        //HCP Income against budget will be calculated through sum of Package_Income

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }

        //if the income is above or equal to 250.00 return true
        public bool IsGreen()
        {
            if (packageIncome >= 250.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //if the income is less than 150.00 return true
        public bool IsRed()
        {
            if (packageIncome < 150.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class OccupancyData {

        public DateTime date { get; set; }
        public int facilityID { get; set; }
        public string careType { get; set; }
        public int occupancy { get; set; }
        public int concessional { get; set; }
        protected bool show = true;


        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public string GetCareType() {
            return careType;
        }

        public int GetOccupancy() {
            return occupancy;
        }

        public int GetConcessional() {
            return concessional;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }

    }

    public class StaffData {

        public int staffID { get; set; }
        public int facilityID { get; set; }
        public string staffFirstName { get; set; }
        public string staffLastName { get; set; }
        public int alAccrued { get; set; }
        public int lslAccrued { get; set; }
        public int slAccrued { get; set; }
        protected bool show = true;

        public int GetStaffID() {
            return staffID;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public string GetStaffFirstName() {
            return staffFirstName;
        }

        public string GetStaffLastName() {
            return staffLastName;
        }

        public int GetAnnualLeaveAcrewed() {
            return alAccrued;
        }

        public int GetLongServiceLeaveAcrewed() {
            return lslAccrued;
        }

        public int GetSickLeaveAcrewed() {
            return slAccrued;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow() {
            return show;
        }

    }

    public class IncomeData {

        public DateTime date { get; set; }
        // protected int InvoiceID;
        public int income { get; set; }
        public int facilityID { get; set; }
        protected bool show = true;


        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public double GetIncome() {
            return income;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }
    }

    public class SalariesWagesData {

        public DateTime date { get; set; }
        public int facilityID { get; set; }
        public int rosteredCost { get; set; }
        public int budget { get; set; }
        protected bool show = true;


        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public double GetRosteredCost() {
            return rosteredCost;
        }

        public double GetBudget() {
            return budget;
        }

        public void Show(bool value)
        {
            show = value;
        }

        public bool GetShow()
        {
            return show;
        }

    }
}