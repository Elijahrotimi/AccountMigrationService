using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Producer.Models
{
    public class AccountDetailsModel
    {
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string description { get; set; }
        public Customer customer { get; set; }
    }
    public class Customer
    {
        public string account_no { get; set; }
        public string create_date { get; set; }
        public string branch_Code { get; set; }
        public string account_type { get; set; }
        public object minor { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string salaryAccount { get; set; }
        public string staff { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string country { get; set; }
        public object designation { get; set; }
        public string maritalStatus { get; set; }
        public string state { get; set; }
        public string allowDebit { get; set; }
        public string allowCredit { get; set; }
        public string allowBlock { get; set; }
        public string stopPay { get; set; }
        public string frozen { get; set; }
        public string gender { get; set; }
        public string title { get; set; }
        public string dormant { get; set; }
        public string taxIdNumber { get; set; }
        public string bvn { get; set; }
        public string accountOpenDate { get; set; }
        public object currentBalance { get; set; }
        public object availableBalance { get; set; }
        public string rmId { get; set; }
        public string ccy { get; set; }
        public string customer_No { get; set; }
        public string account_Class { get; set; }
        public string customer_Type { get; set; }
        public string full_Name { get; set; }
        public string ac_Desc { get; set; }
        public string mobile_Number { get; set; }
        public string kyc_Details { get; set; }
        public string short_Name { get; set; }
        public string email { get; set; }
        public string nationality { get; set; }
        public string customer_Name1 { get; set; }
    }

    public class CustomerReq
    {
        public string Customer_account_no { get; set; }
    }
}
