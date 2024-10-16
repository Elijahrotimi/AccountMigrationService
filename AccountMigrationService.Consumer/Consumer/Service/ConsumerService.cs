using AccountMigrationService.Consumer.Consumer.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountMigrationService.Consumer.Consumer.Service
{
    internal class ConsumerService : IConsumerService
    {
        public void Handle(IModel context, BasicDeliverEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
