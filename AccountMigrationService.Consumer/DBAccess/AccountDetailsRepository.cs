using AccountMigrationService.Consumer.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.DBAccess
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
        public async Task<CustomerModel> RetrieveAccountsInfo(string accountNo)
        {
            _logger.LogInformation($"Begin Retrieve account records for account number {accountNo}");
            var response = new CustomerModel();
            try
            {
                string flexcubeConn = _config.GetConnectionString("FlexcubeDbConnectionString")!;

                string query = DbQueries.GET_NEWLY_ACCOUNT_DETAILS();

                var db = new OracleDataAccess();
                var parameters = new DynamicParameters();
                parameters.Add("accountNo", accountNo);

                IEnumerable<AccountDetailsModel> result = await db.LoadDataWithQuery<AccountDetailsModel, dynamic>(query, parameters, flexcubeConn);

                //response = result.FirstOrDefault();

                if (result.Any())
                {
                    response = GetCustomerModel(result.FirstOrDefault());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving account records for account number {accountNo}");
                return response;
            }
            return response;
        }

        private CustomerModel GetCustomerModel(AccountDetailsModel accountDetailsModel)
        {
            var customerModel = new CustomerModel
            {
                account_no = !string.IsNullOrEmpty(accountDetailsModel.CUST_AC_NO) ? accountDetailsModel.CUST_AC_NO : string.Empty,
                account_type = !string.IsNullOrEmpty(accountDetailsModel.ACCOUNT_TYPE) ? accountDetailsModel.ACCOUNT_TYPE : string.Empty,
                title = !string.IsNullOrEmpty(accountDetailsModel.CUSTOMER_NAME1) ? accountDetailsModel.CUSTOMER_NAME1 : string.Empty,
                branch_Code = !string.IsNullOrEmpty(accountDetailsModel.BRANCH_CODE) ? accountDetailsModel.BRANCH_CODE : string.Empty,
                account_Class = !string.IsNullOrEmpty(accountDetailsModel.ACCOUNT_CLASS) ? accountDetailsModel.ACCOUNT_CLASS : string.Empty,
                customer_No = !string.IsNullOrEmpty(accountDetailsModel.CUST_NO) ? accountDetailsModel.CUST_NO : string.Empty,
                currentBalance = !string.IsNullOrEmpty(accountDetailsModel.ACY_CURR_BALANCE) ? accountDetailsModel.ACY_CURR_BALANCE : string.Empty,
                firstName = !string.IsNullOrEmpty(accountDetailsModel.FIRST_NAME) ? accountDetailsModel.FIRST_NAME : string.Empty,
                middleName = !string.IsNullOrEmpty(accountDetailsModel.MIDDLE_NAME) ? accountDetailsModel.MIDDLE_NAME : string.Empty,
                lastName = !string.IsNullOrEmpty(accountDetailsModel.LAST_NAME) ? accountDetailsModel.LAST_NAME : string.Empty,
                mothersMaidenName = !string.IsNullOrEmpty(accountDetailsModel.MOTHER_MAIDEN_NAME) ? accountDetailsModel.MOTHER_MAIDEN_NAME : string.Empty,
                gender = !string.IsNullOrEmpty(accountDetailsModel.SEX) ? accountDetailsModel.SEX : string.Empty,
                maritalStatus = !string.IsNullOrEmpty(accountDetailsModel.MARITAL_STATUS) ? accountDetailsModel.MARITAL_STATUS : string.Empty,
                address1 = !string.IsNullOrEmpty(accountDetailsModel.ADDRESS_LINE1) ? accountDetailsModel.ADDRESS_LINE1 : string.Empty,
                email = !string.IsNullOrEmpty(accountDetailsModel.E_MAIL) ? accountDetailsModel.E_MAIL : string.Empty,
                short_Name = !string.IsNullOrEmpty(accountDetailsModel.AC_DESC) ? accountDetailsModel.AC_DESC : string.Empty,
                ac_Desc = !string.IsNullOrEmpty(accountDetailsModel.FULL_NAME) ? accountDetailsModel.FULL_NAME : string.Empty,
                customer_Name1 = !string.IsNullOrEmpty(accountDetailsModel.CUSTOMER_NAME1) ? accountDetailsModel.CUSTOMER_NAME1 : string.Empty,
                dateOfBirth = accountDetailsModel.DATE_OF_BIRTH != DateTime.MinValue ? accountDetailsModel.DATE_OF_BIRTH.ToString() : DateTime.Now.ToString(),
                accountOpenDate = accountDetailsModel.AC_OPEN_DATE != DateTime.MinValue ? accountDetailsModel.AC_OPEN_DATE.ToString() : DateTime.Now.ToString(),
                customer_Type = !string.IsNullOrEmpty(accountDetailsModel.CUSTOMER_TYPE) ? accountDetailsModel.CUSTOMER_TYPE : string.Empty,
                bvn = !string.IsNullOrEmpty(accountDetailsModel.BVN) ? accountDetailsModel.BVN : string.Empty,
                nin = !string.IsNullOrEmpty(accountDetailsModel.NIN) ? accountDetailsModel.NIN : string.Empty,
                state = !string.IsNullOrEmpty(accountDetailsModel.STATE_OF_ORIGIN) ? accountDetailsModel.STATE_OF_ORIGIN : string.Empty,
                status = GetFlexCubeAccountStatus(accountDetailsModel)
            };

            return customerModel;
        }
        private string GetFlexCubeAccountStatus(AccountDetailsModel model)
        {
            String status = String.Empty;
            try
            {
                String stat_no_credit = string.IsNullOrEmpty(model.AC_STAT_NO_CR) ? string.Empty : model.AC_STAT_NO_CR.Trim();
                String stat_no_debit = string.IsNullOrEmpty(model.AC_STAT_NO_DR) ? string.Empty : model.AC_STAT_NO_DR.Trim();
                String stat_acct_blocked = string.IsNullOrEmpty(model.AC_STAT_BLOCK) ? string.Empty : model.AC_STAT_BLOCK.Trim();
                String stat_acct_frozen = string.IsNullOrEmpty(model.AC_STAT_FROZEN) ? string.Empty : model.AC_STAT_FROZEN.Trim();
                String stat_acct_stop_pay = string.IsNullOrEmpty(model.AC_STAT_STOP_PAY) ? string.Empty : model.AC_STAT_STOP_PAY.Trim();
                String stat_acct_dormant = string.IsNullOrEmpty(model.AC_STAT_DORMANT) ? string.Empty : model.AC_STAT_DORMANT.Trim();

                if (stat_acct_dormant.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Dormant";
                }
                else if (stat_acct_stop_pay.Equals("Y", StringComparison.OrdinalIgnoreCase) || stat_acct_frozen.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Locked";
                }
                else if (stat_acct_blocked.Equals("Y", StringComparison.OrdinalIgnoreCase) || stat_no_debit.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Restricted";
                }
                else if (stat_no_credit.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Restricted";
                }
                else
                {
                    status = "Active";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetFlexCubeAccountStatus", ex.Message);
            }
            //rs.Close();

            return status;
        }
    }
}
