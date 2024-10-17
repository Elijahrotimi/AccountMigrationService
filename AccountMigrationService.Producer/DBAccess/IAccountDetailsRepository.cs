using AccountMigrationService.Producer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Producer.DBAccess
{
    internal interface IAccountDetailsRepository
    {
        Task<List<NewAccountModel>> RetrieveAccountRecords();
    }
}
