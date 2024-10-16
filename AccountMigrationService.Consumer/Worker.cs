using AccountMigrationService.Consumer.RabbitMqHelper.Interface;

namespace AccountMigrationService.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMq _rabbitMqConnection;

        public Worker(ILogger<Worker> logger, IRabbitMq rabbitMqConnection)
        {
            _logger = logger;
            _rabbitMqConnection = rabbitMqConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"***********{DateTime.Now} Message:******** START PROCESSING*******");
                _rabbitMqConnection.listenForMessage();

            }
            catch (Exception ex)
            {
                _logger.LogError($"*********{DateTime.Now} Message : Process Failed : Message : {ex.Message},\n stack_trace : {ex.StackTrace}, \n Inner Exception : {ex.InnerException}********");
            }
            finally
            {
                await Task.CompletedTask;
            }
        }
    }
}
