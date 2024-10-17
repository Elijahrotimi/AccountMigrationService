using AccountMigrationService.Producer.DBAccess;
using AccountMigrationService.Producer.Models;
using AccountMigrationService.Producer.RabbitMqHelper.Interface;
using AccountMigrationService.Producer.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountMigrationService.Producer.RabbitMqHelper.Service
{
    internal class RabbitMqConnection : IRabbitMq
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly IModel _channel;
        private readonly IAccountDetailsRepository _accountDetailsRepository;

        public RabbitMqConnection(IConfiguration configuration, ILogger<RabbitMqConnection> logger, IModel channel, IAccountDetailsRepository accountDetailsRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _channel = channel;
            _accountDetailsRepository = accountDetailsRepository;
        }
        public async Task ProcessAcctRecords(CancellationToken cancellationToken)
        {
            int delayTimeSpan = _configuration.GetValue<int>("RabbitMQ:DelayTimeSpan");
            InitRabbitMQ();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await BeginMigrationProcess(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Account Migration Service");
                }

                await Task.Delay(TimeSpan.FromSeconds(delayTimeSpan), cancellationToken);
            }
        }

        private async Task BeginMigrationProcess(CancellationToken cancellationToken)
        {
            try
            {
                var records = await _accountDetailsRepository.RetrieveAccountRecords();
                var recordDetails = await _accountDetailsRepository.RetrieveAccountsInfo(records);

                if (recordDetails.Any())
                {
                    Thread.BeginCriticalRegion();
                    foreach (var item in recordDetails)
                    {
                        SendMessageToExchange(item);
                        _logger.LogInformation($"Sent to NEW_ACCOUNT_TOPIC_EXCHANGE:****{item.account_no}****{item.full_Name}");
                    }
                    if (recordDetails.Any())
                    {
                        var lastAccountDate = recordDetails.OrderByDescending(r => r.create_date).FirstOrDefault()!.create_date;
                        TimeStampHandler.UpdateTimeStamp(lastAccountDate);
                    }
                    Thread.EndCriticalRegion();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing new account records");
            }
        }

        private void SendMessageToExchange<T>(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var routingKey = DetermineRoutingKey(message);

            _channel.BasicPublish(exchange: _configuration.GetValue<string>("RabbitMQ:TopicExchangeName"), routingKey: "", basicProperties: null, body: body);
            //_channel.BasicPublish(
            //    exchange: _configuration.GetValue<string>("RabbitMQ:TopicExchangeName"),
            //    routingKey: routingKey,
            //    basicProperties: null,
            //    body: body);
        }
        private string DetermineRoutingKey<T>(T message)
        {
            if (message is Customer accountDetails)
            {
                return string.Format("CUSTOMER.{0}", accountDetails.account_no);
            }
            return "default";
        }

        private void InitRabbitMQ()
        {
            _logger.LogInformation("Initiating RMQ...");
            try
            {
                // create channel  
                _channel.ExchangeDeclare(exchange: _configuration.GetValue<string>("RabbitMQ:TopicExchangeName"), type: ExchangeType.Fanout, durable: false);
                //_channel.ExchangeDeclare(exchange: _configuration.GetValue<string>("RabbitMQ:TopicExchangeName"), type: ExchangeType.Topic, durable: false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Producer Sent {ex.Message}");
            }
        }
    }
}
