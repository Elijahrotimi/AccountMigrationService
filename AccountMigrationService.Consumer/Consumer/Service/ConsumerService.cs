﻿using AccountMigrationService.Consumer.Consumer.Interface;
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
        private readonly IAccountDetailsRepository _accountDetailsRepository;
        private readonly ApplicationSettings _applicationSettings;

        public ConsumerService(ILogger<ConsumerService> logger,
            IOptions<ApplicationSettings> appSettings, IDbUpdateRepository updateRepository, IAccountDetailsRepository accountDetailsRepository)
        {
            _logger = logger;
            _updateRepository = updateRepository;
            _accountDetailsRepository = accountDetailsRepository;
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
                    NewAccountModel model = JsonConvert.DeserializeObject<NewAccountModel>(message)!;
                    if (model != null)
                    {
                        string resp = PerformTask(model).Result;
                        if (resp != null && resp == "Success")
                        {
                            context.BasicAck(eventArgs.DeliveryTag, false);
                        }
                        else
                        {
                            _logger.LogInformation($"Error update records to phoenix - {resp}");
                            context.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Error parsing  {message} into CustomerModel");
                        context.BasicAck(eventArgs.DeliveryTag, false);
                    }
                }
                else
                {
                    _logger.LogInformation($" {message}");
                    context.BasicAck(eventArgs.DeliveryTag, false);
                }
                //context.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing: " + ex.Message);
                if (String.IsNullOrWhiteSpace(message))
                {
                    _logger.LogError(message);
                    context.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
                context.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private async Task<string> PerformTask(NewAccountModel model)
        {
            string response = string.Empty;
            try
            {
                var account_details = await _accountDetailsRepository.RetrieveAccountsInfo(model.account_no);
                if (account_details.account_no != null)
                {
                    response = await _updateRepository.UpdateCustomerRecord(account_details);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured Processing AccountNumber {model.account_no} for transaction with reference with error: {ex.Message}");
            }
            return response;

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
