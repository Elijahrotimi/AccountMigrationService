using AccountMigrationService.Consumer;
using AccountMigrationService.Consumer.Consumer.Interface;
using AccountMigrationService.Consumer.Consumer.Service;
using AccountMigrationService.Consumer.DBAccess;
using AccountMigrationService.Consumer.RabbitMqHelper.Interface;
using AccountMigrationService.Consumer.RabbitMqHelper.Service;
using AccountMigrationService.Consumer.Utilities;
using Serilog;

namespace AccountMigrationService.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostContext, logging) =>
                {
                    var configuration = hostContext.Configuration;
                    string logFolder = configuration.GetSection("ApplicationSettings:Logging:LogPath").Value!;
                    string logFilePath = Path.Combine(logFolder, "AccountMigrationService-.txt");
                    if (!Directory.Exists(logFolder))
                    {
                        Directory.CreateDirectory(logFolder);
                    }
                    Log.Logger = new LoggerConfiguration()
                        //.WriteTo.Console()
                        .WriteTo.File(
                            path: logFilePath, //"Logs/AccountMigrationService-.txt",
                            rollingInterval: RollingInterval.Hour,
                            rollOnFileSizeLimit: true
                        )
                        .CreateLogger();
                })
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ApplicationSettings>(hostContext.Configuration.GetSection(nameof(ApplicationSettings)));
                    services.AddSingleton<IConsumerService, ConsumerService>();
                    services.AddSingleton<IDbUpdateRepository, DbUpdateRepository>();
                    services.AddSingleton<IRabbitMq, RabbitMqConnection>();
                    services.AddHostedService<Worker>();
                    services.AddMemoryCache();
                })
                .Build();
            host.Run();
        }
    }
}