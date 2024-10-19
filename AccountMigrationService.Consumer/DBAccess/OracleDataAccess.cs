using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace AccountMigrationService.Consumer.DBAccess
{
    public class OracleDataAccess
    {

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
                throw;
            }
        }
    }
}
