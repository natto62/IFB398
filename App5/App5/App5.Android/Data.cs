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

    public class BankBalance {

        protected DateTime Date;
        protected double Balance;

        public BankBalance(DateTime dateInput, double balanceInput) {
            this.Date = dateInput;
            this.Balance = balanceInput;
        }

        public double GetBankBalance() {
            return Balance;
        }

        public DateTime GetBankBalanceDate() {
            return Date;
        }

        public void UpdateBankBalance(DateTime dateInput, double balanceInput) {
            this.Date = dateInput;
            this.Balance = balanceInput;
        }


    }

    public class AgencyUsageData {

        protected DateTime Date;
        protected int InvoiceID;
        protected double Amount;
        protected int FacilityID;

        public AgencyUsageData(DateTime dateInput, int invoiceIdInput, double amountInput, int facilityIdInput) {
            this.Date = dateInput;
            this.InvoiceID = invoiceIdInput;
            this.Amount = amountInput;
            this.FacilityID = facilityIdInput;
        }

        public DateTime GetAgencyUsageDate() {
            return Date;
        }

        public int GetAgencyUsageInvoiceID() {
            return InvoiceID;
        }

        public double GetAgencyUsageAmount() {
            return Amount;
        }

        public int GetAgencyUsageFacilityID() {
            return FacilityID;
        }
    }

    public class BrokerageHoursData {

        protected DateTime Date;
        protected int BrokerageID;
        protected int Hours;
        protected int FacilityID;
        //dont know why I put the 'this.Date' instead of just 'Date'
        public BrokerageHoursData(DateTime dateInput, int brokerageIdInput, int hoursInput, int facilityIdInput) {
            this.Date = dateInput;
            this.BrokerageID = brokerageIdInput;
            this.Hours = hoursInput;
            this.FacilityID = facilityIdInput;
        }

    }

    public class HomeCarePackageData {

        protected int ResidentID;
        protected string ResidentFirstName;
        protected string ResidentLastName;
        protected string PackageLevel;
        protected double PackageIncome;

        public HomeCarePackageData(int residentIdInput, string residentFirstNameInput, string residentLastNameInput, string packageLevelInput, double packageIncomeInput) {
            ResidentID = residentIdInput;
            ResidentFirstName = residentFirstNameInput;
            ResidentLastName = residentLastNameInput;
            PackageLevel = packageLevelInput;
            PackageIncome = packageIncomeInput;
        }

    }

    public class OccupancyData {

        protected DateTime Date;
        protected int FacilityID;
        protected int CareTypeID;
        protected int Occupancy;
        protected int Concessional;

        public OccupancyData(DateTime dateInput, int facilityIDInput, int careTypeIDInput, int occupancyInput, int concessionalInput) {
            Date = dateInput;
            FacilityID = facilityIDInput;
            CareTypeID = careTypeIDInput;
            Occupancy = occupancyInput;
            Concessional = concessionalInput;
        }
        

    }

    public class StaffData {

        protected int StaffID;
        protected int FacilityID;
        protected string StaffFirstName;
        protected string StaffLastName;
        protected int AnnualLeaveAcrewed;
        protected int LongServiceLeaveAcrewed;
        protected int ServiceLeaveAcrewed;

        public StaffData(int staffIDInput, int facilityIDInput, string staffFirstNameInput, string staffLastNameInput, int annualLeaveAcrewedInput, int longServiceLeaveAcrewedInput, int serviceLeaveAcrewedInput) {
            StaffID = staffIDInput;
            FacilityID = facilityIDInput;
            StaffFirstName = staffFirstNameInput;
            StaffLastName = staffLastNameInput;
            AnnualLeaveAcrewed = annualLeaveAcrewedInput;
            LongServiceLeaveAcrewed = longServiceLeaveAcrewedInput;
            ServiceLeaveAcrewed = serviceLeaveAcrewedInput;
        }


    }

    public class IncomeData {

        protected DateTime Date;
        protected int FacilityID;
        protected double Income;

        public IncomeData(DateTime dateInput, int facilityIdInput, double incomeInput) {
            Date = dateInput;
            FacilityID = facilityIdInput;
            Income = incomeInput;
        }


    }

    public class SalariesWagesData {

        protected DateTime Date;
        protected int FacilityID;
        protected double RosteredCost;
        protected double Budget;

        public SalariesWagesData(DateTime dateInput, int facilityIdInput, double rosteredCostInput, double budgetInput) {
            Date = dateInput;
            FacilityID = facilityIdInput;
            RosteredCost = rosteredCostInput;
            Budget = budgetInput;
        }

    }
}