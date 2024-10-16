using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.Utilities
{
    public class ApplicationSettings
    {
        public Rabbitmq RabbitMq { get; set; }
        public Rabbitmqexchange RabbitMqExchange { get; set; }
    }

    public class Rabbitmq
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ushort FetchCount { get; set; }
    }

    public class Rabbitmqexchange
    {
        public string Type { get; set; }
        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public bool RequeueFailedMessages { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
    }
}
