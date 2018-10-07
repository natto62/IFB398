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
        public decimal income { get; set; }
        public DateTime date { get; set; }
        public DateTime expiration { get; set; }
        protected bool show = true;

        public DateTime GetDate() {
            return date;
        }

        public DateTime GetExpirationDate() {
            return expiration;
        }

        public int GetResidentID() {
            return residentID;
        }

        public string GetACFIScore() {
            return acfiScore;
        }

        public decimal GetIncome() {
            return income;
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

    public class BankBalance {

        public DateTime date { get; set; }
        public decimal balance { get; set; }
        //public int facilityID { get; set; }
        protected bool show = true;


        public decimal GetBankBalance() {
            return balance;
        }

        public DateTime GetDate() {
            return date;
        }

        //public int GetFacilityID()
        //{
        //    return facilityID;
        //}

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
        public decimal Amount { get; set; }
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

        public decimal GetAgencyUsageAmount() {
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
        public decimal hours { get; set; }
        public int facilityID { get; set; }
        public string location { get; set; }
        protected bool show = true;

        public DateTime GetDate() {
            return date;
        }

        //public int GetBrokerageID() {
        //    return BrokerageID;
        //}

        public string GetLocation() {
            return location;
        }

        public decimal GetBrokerageHours() {
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
        public decimal packageIncome { get; set; }
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

        public decimal GetPackageIncome() {
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
            decimal greenVal = 250;
            if (Decimal.Compare(packageIncome, greenVal) >= 0)
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
            decimal redVal = 150;
            if (Decimal.Compare(packageIncome, redVal) < 0)
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
        public int totalbeds { get; set; }
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

        public int GetActualBeds() {
            return occupancy;
        }

        public int GetSupported() {
            return concessional;
        }

        public int GetTotalBeds() {
            return totalbeds;
        }

        public int GetTotalBedDays() {
            int totalBedDays = 0;
            int year = DateTime.Now.Year;
            if (DateTime.IsLeapYear(year)) {//leap year
                totalBedDays = totalbeds * 366;
            } else {
                totalBedDays = totalbeds * 365;
            }
            return totalBedDays;
        }

        public double GetOccupancyRate() {
            int bedDays = GetTotalBedDays();
            double OccupancyRate = 0.0;
            OccupancyRate = bedDays / (30 * (totalbeds - occupancy - concessional));
            return OccupancyRate;
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
        public decimal alAccrued { get; set; }
        public decimal lslAccrued { get; set; }
        public decimal slAccrued { get; set; }
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

        public decimal GetAnnualLeaveAcrewed() {
            return alAccrued;
        }

        public decimal GetLongServiceLeaveAcrewed() {
            return lslAccrued;
        }

        public decimal GetSickLeaveAcrewed() {
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
        public decimal totalPackageIncome { get; set; }
        public decimal businessService { get; set; }
        public decimal settlementService { get; set; }
        public string facilityName { get; set; }
        public string location { get; set; }
        public int facilityID { get; set; }
        protected static string type = "";
        protected bool show = true;


        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public void SetType(string typeVal) {
            type = typeVal;
        }

        public decimal GetIncome() {
            decimal returnVal = 0;
            if (String.Equals(type,"Total Package Income")) {
                returnVal = totalPackageIncome;
            } else if (String.Equals(type, "Business Service")) {
                returnVal = businessService;
            } else if (String.Equals(type, "Settlement Service")) {
                returnVal = settlementService;
            }
            return returnVal;
        }

        public string GetFacilityName() {
            return facilityName;
        }

        public string GetLocation() {
            return location;
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
        public decimal rosteredCost { get; set; }
        public decimal budget { get; set; }
        protected bool show = true;


        public DateTime GetDate() {
            return date;
        }

        public int GetFacilityID() {
            return facilityID;
        }

        public decimal GetActualCost() {
            return rosteredCost;
        }

        public decimal GetBudget() {
            return budget;
        }

        public decimal GetVariance() {
            return (budget - rosteredCost);
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