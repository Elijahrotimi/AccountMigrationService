using AccountMigrationService.Producer.DBAccess;
using AccountMigrationService.Producer.RabbitMqHelper.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

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
            //InitRabbitMQ();
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
            var records = await _accountDetailsRepository.RetrieveAccountRecords();
            throw new NotImplementedException();
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
