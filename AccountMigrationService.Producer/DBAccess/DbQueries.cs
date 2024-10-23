using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Producer.DBAccess
{
    public class DbQueries
    {
        public static string GET_NEWLY_OPENED_ACCOUNTS()
        {
            var query = "select CUST_AC_NO account_no, To_Char(CHECKER_DT_STAMP, 'DD-MON-YYYY HH:MI:SS') create_date from zenithubs.sttm_cust_account where To_Char(CHECKER_DT_STAMP, 'DD-MON-YYYY HH:MI:SS') > :lastDate and maker_id <>'MIGRATION' and AC_OPEN_DATE >= '01-OCT-2024'";
            return query;
        }
    }
}
