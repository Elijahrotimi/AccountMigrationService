using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.Models
{
    public class AccountDetailsModel
    {
        public string BRANCH_CODE { get; set; }
        public string ACCOUNT_TYPE { get; set; }
        public string AC_DESC { get; set; }
        public string CUST_NO { get; set; }
        public string CCY { get; set; }
        public string CUST_AC_NO { get; set; }
        public string ACCOUNT_CLASS { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_NAME1 { get; set; }
        public string FULL_NAME { get; set; }
        public string NATIONALITY { get; set; }
        public DateTime AC_OPEN_DATE { get; set; }
        public string ACY_CURR_BALANCE { get; set; }
        public string AC_STAT_NO_DR { get; set; }
        public string AC_STAT_NO_CR { get; set; }
        public string AC_STAT_BLOCK { get; set; }
        public string AC_STAT_STOP_PAY { get; set; }
        public string AC_STAT_FROZEN { get; set; }
        public string ADDRESS_LINE1 { get; set; }
        public string ADDRESS_LINE2 { get; set; }
        public string FIRST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public DateTime DATE_OF_BIRTH { get; set; }
        public string CUSTOMER_PREFIX { get; set; }
        public string MOBILE_NUMBER { get; set; }
        public string E_MAIL { get; set; }
        public string SEX { get; set; }
        public string MOTHER_MAIDEN_NAME { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string ACY_BLOCKED_AMT { get; set; }
        public string ACY_WITHDRAWABLE_BAL { get; set; }
        public string AC_STAT_DORMANT { get; set; }
        public string BVN { get; set; }
    }
}
