using AccountMigrationService.Producer.Models;
using AccountMigrationService.Producer.Utilities;
using Dapper;
using Newtonsoft.Json;
using RestSharp;

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
                string flexcubeConn = _config.GetConnectionString("FlexcubeDbConnectionString")!;
                string lastDate = TimeStampHandler.GetTimeStamp();

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

        public async Task<List<Customer>> RetrieveAccountsInfo(List<NewAccountModel> records)
        {
            List<Customer> accounts = new List<Customer>();
            try
            {
                Parallel.ForEach(records, record =>
                   {
                       var accountDetails = Task.Run(async () => await GetCustomerInfo(record.account_no)).Result.customer;
                       if (accountDetails != null && !string.IsNullOrEmpty(accountDetails.firstName))
                       {
                           accountDetails.account_no = record.account_no;
                           accountDetails.create_date = record.create_date;
                       }
                       else
                       {
                           accountDetails = new Customer
                           {
                               account_no = record.account_no,
                               create_date = record.create_date
                           };
                       }

                       lock (accountDetails) ;
                       accounts.Add(accountDetails);
                   });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return accounts;
        }

        private async Task<AccountDetailsModel> GetCustomerInfo(string accountNo)
        {
            _logger.LogInformation("Calling GetCustomerInfo endpoint... : " + accountNo);

            var baseUrl = _config["FCubeAccount"];
            AccountDetailsModel accountDetailsModel = new AccountDetailsModel();

            try
            {
                var options = new RestClientOptions(baseUrl)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest("", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                CustomerReq customer = new CustomerReq();
                customer.Customer_account_no = accountNo;
                request.AddJsonBody(customer);
                RestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("CustomerInfo Api Call responsed with : " + response.Content);
                    accountDetailsModel = JsonConvert.DeserializeObject<AccountDetailsModel>(response.Content);

                    return accountDetailsModel;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

            }
            return accountDetailsModel;

        }
    }
}
