namespace AccountMigrationService.Producer.RabbitMqHelper.Interface
{
    public interface IRabbitMq
    {
        Task ProcessAcctRecords(CancellationToken cancellationToken);
    }
}
