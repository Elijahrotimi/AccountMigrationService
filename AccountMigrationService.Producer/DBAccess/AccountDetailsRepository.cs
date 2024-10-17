using AccountMigrationService.Producer.Models;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace AccountMigrationService.Producer.DBAccess
{
    public class AccountDetailsRepository : IAccountDetailsRepository
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AccountDetailsRepository> _logger;

        public AccountDetailsRepository(IConfiguration config, ILogger<AccountDetailsRepository> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<List<NewAccountModel>> RetrieveAccountRecords()
        {
            _logger.LogInformation("Begin Retrieve account records...");
            var response = new List<NewAccountModel>();
            try
            {
                string flexcubeConn = _config.GetConnectionString("FlexcubeDbConnectionString3")!;
                string lastDate = "01-OCT-2024 10:55:58";

                string query = DbQueries.GET_NEWLY_OPENED_ACCOUNTS();

                var db = new OracleDataAccess();
                var parameters = new DynamicParameters();
                parameters.Add("lastDate", lastDate);

                IEnumerable<NewAccountModel> result = await db.LoadDataWithQuery<NewAccountModel, dynamic>(query, parameters, flexcubeConn);

                response = result.ToList();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving account records");
                return response;
            }
            return response;
        }
    }
}
