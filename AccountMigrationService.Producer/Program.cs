using AccountMigrationService.Producer;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using Serilog;
using AccountMigrationService.Producer.RabbitMqHelper.Interface;
using AccountMigrationService.Producer.RabbitMqHelper.Service;
using AccountMigrationService.Producer.DBAccess;

namespace AccountMigrationService.Producer;
public class Program
{
    private static IConfiguration configuration = null!;

    static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostContext, logging) =>
            {
                configuration = hostContext.Configuration;
                string logFolder = configuration.GetSection("Logging:LogPath").Value!;
                string logFilePath = Path.Combine(logFolder, "AccountMigrationService-.txt");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    //.WriteTo.Console()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                    .CreateLogger();
            })
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<IModel>((serviceProvider) =>
                {
                    try
                    {
                        // Log RabbitMQ configuration values
                        var rabbitMQHost = configuration.GetValue<string>("RabbitMQ:RMQHost");
                        var rabbitMQPort = configuration.GetValue<int>("RabbitMQ:RMQPort");
                        var rabbitMQUsername = configuration.GetValue<string>("RabbitMQ:RMQUsername");
                        var rabbitMQPassword = configuration.GetValue<string>("RabbitMQ:RMQPassword");

                        Log.Logger.Information("RabbitMQ configuration:");
                        Log.Logger.Information($"HostName: {rabbitMQHost}");
                        Log.Logger.Information($"Port: {rabbitMQPort}");
                        Log.Logger.Information($"Username: {rabbitMQUsername}");

                        var factory = new ConnectionFactory
                        {
                            HostName = rabbitMQHost,
                            Port = rabbitMQPort,
                            UserName = rabbitMQUsername,
                            Password = rabbitMQPassword
                        };

                        var connection = factory.CreateConnection();
                        var channel = connection.CreateModel();
                        serviceProvider.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
                        {
                            // Release resources on application shutdown
                            channel.Close();
                            connection.Close();
                        });

                        Log.Logger.Information("RabbitMQ connection established successfully.");
                        return channel;
                    }
                    catch (BrokerUnreachableException ex)
                    {
                        // Log the error and terminate application startup
                        Log.Logger.Error(ex, "Failed to connect to RabbitMQ broker: {Message}", ex.Message);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        // Log any other unexpected exceptions
                        Log.Logger.Error(ex, "An unexpected error occurred while connecting to RabbitMQ: {Message}", ex.Message);
                        throw;
                    }
                });

                services.AddSingleton<IRabbitMq, RabbitMqConnection>();
                services.AddSingleton<IAccountDetailsRepository, AccountDetailsRepository>();
            })
            .UseSerilog()
            .UseWindowsService()
            .Build();

        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An unhandled exception occurred during application execution: {Message}", ex.Message);
            throw;
        }
    }
}

