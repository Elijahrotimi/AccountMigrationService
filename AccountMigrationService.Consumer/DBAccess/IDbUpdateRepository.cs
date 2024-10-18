using AccountMigrationService.Consumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.DBAccess
{
    public interface IDbUpdateRepository
    {
        Task<string> UpdateCustomerRecord(CustomerModel customer);
    }
}
