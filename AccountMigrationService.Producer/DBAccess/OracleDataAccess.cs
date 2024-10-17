using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace AccountMigrationService.Producer.DBAccess
{
    public class OracleDataAccess
    {
        //private readonly ILogger<OracleDataAccess> _logger;

        //public OracleDataAccess(ILogger<OracleDataAccess> logger)
        //{
        //    _logger = logger;
        //}
        public Task<IEnumerable<T>> LoadDataWithQuery<T, U>(string query, U parameters, string connectionId = "")
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(connectionId))
                {
                    var result = connection.QueryAsync<T>(query, parameters, commandType: CommandType.Text);

                    return result;
                }


            }
            catch (Exception ex)
            {
                //_logger.LogError("OracleDataAccess / LoadDataWithQuery / Error : " + ex.Message);

                throw;
            }
        }
    }
}
