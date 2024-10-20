using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.DBAccess
{
    internal class DbQueries
    {
     //   public static string GET_NEWLY_ACCOUNT_DETAILS()
     //   {
     //       var query = @"SELECT  
     //               v.CUSTOMER_NO, 
     //               c.CUSTOMER_TYPE,  
     //               c.CUSTOMER_NAME1,  
     //               c.FULL_NAME,  
     //               c.SHORT_NAME,  
     //               c.NATIONALITY,  
     //               c.KYC_DETAILS,  
     //               c.STAFF,  
     //               c.ADDRESS_LINE1,  
     //               c.ADDRESS_LINE2,  
     //               c.ADDRESS_LINE3,  
     //               c.ADDRESS_LINE4,  
     //               v.MOBILE_NUMBER,  
     //               v.E_MAIL,  
     //               v.SEX,  
     //               (SELECT e.MARITAL_STATUS FROM ZENITHUBS.STTM_CUST_DOMESTIC e WHERE c.CUSTOMER_NO =e.CUSTOMER_NO) AS  MARITAL_STATUS,
     //               v.MINOR,  
     //               c.RM_ID,  
     //               v.FIRST_NAME,  
     //               v.MIDDLE_NAME,  
     //               v.LAST_NAME,  
     //               v.DATE_OF_BIRTH,  
     //               v.CUSTOMER_PREFIX,
					//v.TELEPHONE,
					//(SELECT h.BVN FROM ZENITHUBS.STZM_STDCIF_KYC_CUSTOM h WHERE c.CUSTOMER_NO = h.CUST_NO) AS BVN,
					//(SELECT h.NIN FROM ZENITHUBS.STZM_STDCIF_KYC_CUSTOM h WHERE c.CUSTOMER_NO = h.CUST_NO) AS NIN
     //               FROM  ZENITHUBS.STTM_CUSTOMER c
     //               LEFT JOIN ZENITHUBS.STTM_CUST_PERSONAL v  ON  c.CUSTOMER_NO =v.CUSTOMER_NO 
     //               WHERE c.CUSTOMER_NO=:accountNo";
     //       return query;
     //   }
        public static string GET_NEWLY_ACCOUNT_DETAILS()
        {
            var query = @"SELECT 
                b.Branch_Code,
	            b.ACCOUNT_TYPE,
                b.AC_DESC,
                b.CUST_NO,
                b.CCY,
                b.CUST_AC_NO,
                b.ACCOUNT_CLASS,
                a.CUSTOMER_TYPE,
                a.CUSTOMER_NAME1,
                a.FULL_NAME,
                a.NATIONALITY,
                f.AC_OPEN_DATE,
                f.ACY_CURR_BALANCE,
                f.AC_STAT_NO_DR,
                f.AC_STAT_NO_CR,
                f.AC_STAT_BLOCK,
                f.AC_STAT_STOP_PAY,
                f.AC_STAT_FROZEN,
                a.ADDRESS_LINE1,  
                a.ADDRESS_LINE2, 
                v.FIRST_NAME,  
                v.MIDDLE_NAME,  
                v.LAST_NAME,  
                v.DATE_OF_BIRTH,  
                v.CUSTOMER_PREFIX, 
                v.MOBILE_NUMBER,  
                v.E_MAIL,  
                v.SEX, 
                v.MOTHER_MAIDEN_NAME,
                (SELECT e.MARITAL_STATUS FROM ZENITHUBS.STTM_CUST_DOMESTIC e WHERE a.CUSTOMER_NO =e.CUSTOMER_NO) AS  MARITAL_STATUS,  
                (SELECT g.ACY_BLOCKED_AMT FROM ZENITHUBS.STTM_ACCOUNT_BALANCE g WHERE b.CUST_AC_NO=g.CUST_AC_NO) AS ACY_BLOCKED_AMT,
                (SELECT g.ACY_WITHDRAWABLE_BAL FROM ZENITHUBS.STTM_ACCOUNT_BALANCE g WHERE b.CUST_AC_NO =g.CUST_AC_NO) AS ACY_WITHDRAWABLE_BAL,
                (SELECT g.AC_STAT_DORMANT FROM ZENITHUBS.STTM_ACCOUNT_BALANCE g WHERE b.CUST_AC_NO =g.CUST_AC_NO) AS AC_STAT_DORMANT,
                (SELECT h.BVN FROM ZENITHUBS.STZM_STDCIF_KYC_CUSTOM h WHERE b.CUST_NO = h.CUST_NO) AS BVN,
                (SELECT h.STATE_OF_ORIGIN FROM ZENITHUBS.STZM_STDCIF_KYC_CUSTOM h WHERE b.CUST_NO = h.CUST_NO) AS STATE_OF_ORIGIN,
                (SELECT h.NIN FROM ZENITHUBS.STZM_STDCIF_KYC_CUSTOM h WHERE b.CUST_NO = h.CUST_NO) AS NIN
                FROM ZENITHUBS.STTM_CUST_ACCOUNT b
                JOIN 
                ZENITHUBS.sttm_customer a ON b.CUST_NO = a.CUSTOMER_NO
                LEFT JOIN 
                ZENITHUBS.STTM_CUST_ACCOUNT f ON f.CUST_AC_NO = b.CUST_AC_NO
                LEFT JOIN ZENITHUBS.STTM_CUST_PERSONAL v  ON  a.CUSTOMER_NO =v.CUSTOMER_NO 
                WHERE  b.CUST_AC_NO =:accountNo";
            return query;
        }
    }
}
