using AccountMigrationService.Producer.RabbitMqHelper.Interface;

namespace AccountMigrationService.Producer
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
                _logger.LogInformation("Entering Worker running at: {time}", DateTimeOffset.Now);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await _rabbitMqConnection.ProcessAcctRecords(stoppingToken);
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "An error occurred in the Worker");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {Message}", ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
