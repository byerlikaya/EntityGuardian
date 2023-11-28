namespace EntityGuardian.BackgroundServices;

public class DataBackgroundService(IServiceProvider serviceProvider,
    EntityGuardianOption configuration,
    ILogger<DataBackgroundService> logger) : BackgroundService
{
    private DateTime _nextRunTime = DateTime.UtcNow;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("EntityGuardian Background Service is starting.");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("EntityGuardian Background Service is working.");

                await Task.Delay(GetDelayTime(), cancellationToken);

                using (var scope = serviceProvider.CreateScope())
                {
                    var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
                    await storageService.Synchronization(cancellationToken);
                }

                _nextRunTime = GetNextDate();
            }
            catch (Exception exception)
            {
                logger.LogError($"EntityGuardian Background Service encountered an error. Error Message : {exception.Message}");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("EntityGuardian Background Service has been stopped.");
        return base.StopAsync(cancellationToken);
    }

    private int GetDelayTime()
    {
        var delayTime = DelayTime();

        if (delayTime > 0)
            return (int)delayTime;

        _nextRunTime = GetNextDate();

        return (int)DelayTime();
    }

    private DateTime GetNextDate() => DateTime.UtcNow.AddSeconds(configuration.DataSynchronizationTimeout);

    private double DelayTime() => (_nextRunTime - DateTime.UtcNow).TotalMilliseconds;
}