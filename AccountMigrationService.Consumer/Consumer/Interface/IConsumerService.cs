using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.Consumer.Interface
{
    public interface IConsumerService
    {
        void Handle(IModel context, BasicDeliverEventArgs args);
    }
}
