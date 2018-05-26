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

        public BankBalance(DateTime Date, double Balance) {
            this.Date = Date;
            this.Balance = Balance;
        }

        public double GetBankBalance() {
            return Balance;
        }

        public DateTime GetDate() {
            return Date;
        }

        public void UpdateBankBalance(DateTime Date, double Balance) {
            this.Date = Date;
            this.Balance = Balance;
        }


    }

    public class AgencyUsageData {

        protected DateTime Date;
        protected int InvoiceID;
        protected double Amount;
        protected int FacilityID;

        public AgencyUsageData(DateTime Date, int InvoiceID, double Amount, int FacilityID) {
            this.Date = Date;
            this.InvoiceID = InvoiceID;
            this.Amount = Amount;
            this.FacilityID = FacilityID;
        }

        public DateTime GetDate() {
            return Date;
        }

        public int GetInvoiceID() {
            return InvoiceID;
        }

        public double GetAgencyUsageAmount() {
            return Amount;
        }

        public int GetFacilityID() {
            return FacilityID;
        }
    }

    public class BrokerageHoursData {

        protected DateTime Date;
        protected int BrokerageID;
        protected int Hours;
        protected int FacilityID;

        public BrokerageHoursData(DateTime Date, int BrokerageID, int Hours, int FacilityID) {
            this.Date = Date;
            this.BrokerageID = BrokerageID;
            this.Hours = Hours;
            this.FacilityID = FacilityID;
        }

        public DateTime GetDate() {
            return Date;
        }

        public int GetBrokerageID() {
            return BrokerageID;
        }

        public int GetBrokerageHours() {
            return Hours;
        }

        public int GetFacilityID() {
            return FacilityID;
        }
    }

    public class HomeCarePackageData {

        protected int ResidentID;
        protected string ResidentFirstName;
        protected string ResidentLastName;
        protected string PackageLevel;
        protected double PackageIncome;

        public HomeCarePackageData(int ResidentID, string ResidentFirstName, string ResidentLastName, string PackageLevel, double PackageIncome) {
            this.ResidentID = ResidentID;
            this.ResidentFirstName = ResidentFirstName;
            this.ResidentLastName = ResidentLastName;
            this.PackageLevel = PackageLevel;
            this.PackageIncome = PackageIncome;
        }

        public int GetResidentID() {
            return ResidentID;
        }

        public string GetResidentFirstName() {
            return ResidentFirstName;
        }

        public string GetResidentLastName() {
            return ResidentLastName;
        }

        public string GetPackageLevel() {
            return PackageLevel;
        }

        public double GetPackageIncome() {
            return PackageIncome;
        }
    }

    public class OccupancyData {

        protected DateTime Date;
        protected int FacilityID;
        protected int CareTypeID;
        protected int Occupancy;
        protected int Concessional;

        public OccupancyData(DateTime Date, int FacilityID, int CareTypeID, int Occupancy, int Concessional) {
            this.Date = Date;
            this.FacilityID = FacilityID;
            this.CareTypeID = CareTypeID;
            this.Occupancy = Occupancy;
            this.Concessional = Concessional;
        }

        public DateTime GetDate() {
            return Date;
        }

        public int GetFacilityID() {
            return FacilityID;
        }

        public int GetCareTypeID() {
            return CareTypeID;
        }

        public int GetOccupancy() {
            return Occupancy;
        }

        public int GetConcessional() {
            return Concessional;
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

        public StaffData(int StaffID, int FacilityID, string StaffFirstName, string StaffLastName, int AnnualLeaveAcrewed, int LongServiceLeaveAcrewed, int ServiceLeaveAcrewed) {
            this.StaffID = StaffID;
            this.FacilityID = FacilityID;
            this.StaffFirstName = StaffFirstName;
            this.StaffLastName = StaffLastName;
            this.AnnualLeaveAcrewed = AnnualLeaveAcrewed;
            this.LongServiceLeaveAcrewed = LongServiceLeaveAcrewed;
            this.ServiceLeaveAcrewed = ServiceLeaveAcrewed;
        }

        public int GetStaffID() {
            return StaffID;
        }

        public int GetFacilityID() {
            return FacilityID;
        }

        public string GetStaffFirstName() {
            return StaffFirstName;
        }

        public string GetStaffLastName() {
            return StaffLastName;
        }

        public int GetAnnualLeaveAcrewed() {
            return AnnualLeaveAcrewed;
        }

        public int GetLongServiceLeaveAcrewed() {
            return LongServiceLeaveAcrewed;
        }

        public int GetServiceLeaveAcrewed() {
            return ServiceLeaveAcrewed;
        }

    }

    public class IncomeData {

        protected DateTime Date;
        protected int FacilityID;
        protected double Income;

        public IncomeData(DateTime Date, int FacilityID, double Income) {
            this.Date = Date;
            this.FacilityID = FacilityID;
            this.Income = Income;
        }

        public DateTime GetDate() {
            return Date;
        }

        public int GetFacilityID() {
            return FacilityID;
        }

        public double GetIncome() {
            return Income;
        }
    }

    public class SalariesWagesData {

        protected DateTime Date;
        protected int FacilityID;
        protected double RosteredCost;
        protected double Budget;

        public SalariesWagesData(DateTime Date, int FacilityID, double RosteredCost, double Budget) {
            this.Date = Date;
            this.FacilityID = FacilityID;
            this.RosteredCost = RosteredCost;
            this.Budget = Budget;
        }

        public DateTime GetDate() {
            return Date;
        }

        public int GetFacilityID() {
            return FacilityID;
        }

        public double GetRosteredCost() {
            return RosteredCost;
        }

        public double GetBudget() {
            return Budget;
        }

    }
}