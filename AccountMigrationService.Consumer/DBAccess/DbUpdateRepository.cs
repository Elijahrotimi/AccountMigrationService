using AccountMigrationService.Consumer.Models;
using AdoNetCore.AseClient;
using System.Data;

namespace AccountMigrationService.Consumer.DBAccess
{
    public class DbUpdateRepository : IDbUpdateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbUpdateRepository> _logger;

        public DbUpdateRepository(IConfiguration configuration, ILogger<DbUpdateRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<string> UpdateCustomerRecord(CustomerModel customer)
        {
            string response = string.Empty;
            string connectionstring = _configuration.GetConnectionString("ZenbaseConnectionString")!;
            using AseConnection connection = new AseConnection(connectionstring);
            try
            {
                using (AseCommand command = new AseCommand("zsp_dp_acct_Max_bal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@psAccountNumber",         customer.account_no);
                    command.Parameters.AddWithValue("@psAccountType",           customer.account_type);
                    command.Parameters.AddWithValue("@psStatus",                customer.status);
                    command.Parameters.AddWithValue("@psTitle1",                customer.title);
                    command.Parameters.AddWithValue("@psTitle2",                customer.customer_Name1);
                    command.Parameters.AddWithValue("@psBranchNumber",          customer.branch_Code);
                    command.Parameters.AddWithValue("@psClassCode",             customer.account_Class);
                    command.Parameters.AddWithValue("@psRimNumber",             customer.customer_No);
                    command.Parameters.AddWithValue("@pnCurrentBalance",        Decimal.Parse(customer.currentBalance));

                    command.Parameters.AddWithValue("@psFirstName",             customer.firstName);
                    command.Parameters.AddWithValue("@psLastName",              customer.lastName);
                    command.Parameters.AddWithValue("@pdtDateOfBirth",          DateTime.Parse(customer.dateOfBirth));
                    command.Parameters.AddWithValue("@psMotherMaidenName",      customer.account_no);
                    command.Parameters.AddWithValue("@psSex",                   customer.gender);
                    command.Parameters.AddWithValue("@psMaritalStatus",         customer.maritalStatus);
                    command.Parameters.AddWithValue("@psNextOfKin",             customer.account_no);
                    command.Parameters.AddWithValue("@psIdType",                string.Empty);
                    command.Parameters.AddWithValue("@psIdNumber",              string.Empty);
                    command.Parameters.AddWithValue("@psMailingAddress",        customer.address1);
                    command.Parameters.AddWithValue("@psAccountHolderType",     customer.customer_Type);
                    command.Parameters.AddWithValue("@psAccountName",           customer.short_Name);
                    command.Parameters.AddWithValue("@psBvn",                   customer.bvn);
                    command.Parameters.AddWithValue("@psNin",                   customer.nin);
                    command.Parameters.AddWithValue("@psCity",                  string.Empty);
                    command.Parameters.AddWithValue("@psState",                 customer.state);

                    connection.Open();
                    await command.ExecuteReaderAsync();

                    response = "Success";

                    command.Dispose();
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured :{ex.Message}");
                response = ex.Message;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            return response;
        }
    }
}
