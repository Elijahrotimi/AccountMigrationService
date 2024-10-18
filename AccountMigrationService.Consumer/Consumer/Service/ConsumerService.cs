using AccountMigrationService.Consumer.Consumer.Interface;
using AccountMigrationService.Consumer.DBAccess;
using AccountMigrationService.Consumer.Models;
using AccountMigrationService.Consumer.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace AccountMigrationService.Consumer.Consumer.Service
{
    public class ConsumerService : IConsumerService
    {
        private int ProceesedJson = 0;
        private readonly ILogger<ConsumerService> _logger;
        private readonly IDbUpdateRepository _updateRepository;
        private readonly ApplicationSettings _applicationSettings;

        public ConsumerService(ILogger<ConsumerService> logger,
            IOptions<ApplicationSettings> appSettings, IDbUpdateRepository updateRepository)
        {
            _logger = logger;
            _updateRepository = updateRepository;
            _applicationSettings = appSettings.Value;
        }
        public void Handle(IModel context, BasicDeliverEventArgs eventArgs)
        {
            String message = String.Empty;
            var body = eventArgs.Body.ToArray();
            try
            {
                message = Encoding.UTF8.GetString(body);
                if (IsValidJson(message))
                {
                    CustomerModel model = JsonConvert.DeserializeObject<CustomerModel>(message)!;
                    if (model != null)
                    {
                        _ = PerformTask(model);
                    }
                    else
                    {
                        _logger.LogInformation($"Error parsing  {message} into CustomerModel");
                    }
                }
                else
                {
                    _logger.LogInformation($" {message}");
                }
                context.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing" + ex.Message);
                if (String.IsNullOrWhiteSpace(message))
                {
                    _logger.LogError(message);
                }
            }
        }

        private async Task PerformTask(CustomerModel model)
        {
            try
            {
                await _updateRepository.UpdateCustomerRecord(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured Processing AccountNumber {model.account_no} for transaction with reference with error: {ex.Message}");
            }

        }
        private bool IsValidJson(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                ProceesedJson++;
                return true;
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError($"Invalid Json: {ex.Message} {jsonString}");
                return false;
            }
        }
    }
}
