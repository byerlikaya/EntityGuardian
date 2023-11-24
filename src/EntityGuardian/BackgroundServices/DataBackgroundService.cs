namespace EntityGuardian.BackgroundServices;

public class DataBackgroundService : BackgroundService
{
    private readonly IStorageService _storageService;
    private readonly ILogger<DataBackgroundService> _logger;
    private readonly EntityGuardianOption _configuration;
    private DateTime _nextRunTime = DateTime.UtcNow;


    public DataBackgroundService(EntityGuardianOption configuration, IStorageService storageService, ILogger<DataBackgroundService> logger)
    {
        _configuration = configuration;
        _storageService = storageService;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("EntityGuardian Background Service is starting.");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("EntityGuardian Background Service is working.");

                await Task.Delay(GetDelayTime(), cancellationToken);

                await _storageService.Synchronization(cancellationToken);

                _nextRunTime = GetNextDate();
            }
            catch (Exception exception)
            {
                _logger.LogError($"EntityGuardian Background Service encountered an error. Error Message : {exception.Message}");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("EntityGuardian Background Service has been stopped.");
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

    private DateTime GetNextDate() => DateTime.UtcNow.AddSeconds(_configuration.DataSynchronizationTimeout);

    private double DelayTime() => (_nextRunTime - DateTime.UtcNow).TotalMilliseconds;
}