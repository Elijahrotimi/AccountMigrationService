using AccountMigrationService.Consumer.Consumer.Interface;
using AccountMigrationService.Consumer.RabbitMqHelper.Interface;
using AccountMigrationService.Consumer.Utilities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.RabbitMqHelper.Service
{
    public class RabbitMqConnection : IRabbitMq
    {
        private readonly IConsumerService _consumerService;
        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly ApplicationSettings _applicationSettings;
        private IConnection _connection;
        private IModel channel = null;

        public RabbitMqConnection(
            ILogger<RabbitMqConnection> logger,
            IOptions<ApplicationSettings> options
            )
        {
            this._logger = logger;
            _applicationSettings = options.Value;
        }
        public void listenForMessage()
        {
            var factory = new ConnectionFactory
            {
                HostName = _applicationSettings.RabbitMq.HostName,
                Port = _applicationSettings.RabbitMq.Port,
                UserName = _applicationSettings.RabbitMq.UserName,
                Password = _applicationSettings.RabbitMq.Password
            };
            _connection = factory.CreateConnection();
            channel = _connection.CreateModel();
            channel.BasicQos(0, _applicationSettings.RabbitMq.FetchCount, false);
            channel.QueueDeclare(queue: _applicationSettings.RabbitMqExchange.QueueName, durable: true, exclusive: false, autoDelete: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                _consumerService.Handle(channel, eventArgs);
            };
            var consumerTag = channel.BasicConsume(queue: _applicationSettings.RabbitMqExchange.QueueName, autoAck: false, consumer: consumer);
        }


    }
}
