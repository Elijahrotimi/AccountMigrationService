using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Consumer.RabbitMqHelper.Interface
{
    public interface IRabbitMq
    {
        void listenForMessage();
    }
}
