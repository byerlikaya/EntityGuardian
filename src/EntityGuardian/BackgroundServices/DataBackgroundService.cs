using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityGuardian.BackgroundServices
{
    public class DataBackgroundService : BackgroundService
    {

        private readonly EntityGuardianOption _configuration = ServiceTool.ServiceProvider.GetService<EntityGuardianOption>();
        private DateTime _nextRunTime = DateTime.UtcNow;
        private readonly IStorageService _storageService = ServiceTool.ServiceProvider.GetService<IStorageService>();

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelayTime(), cancellationToken);
                await _storageService.Synchronization();
                _nextRunTime = GetNextDate();
            }
        }

        private int GetDelayTime()
        {
            var delayTime = (_nextRunTime - DateTime.UtcNow).TotalMilliseconds;

            if (!(delayTime < 0))
                return (int)delayTime;

            _nextRunTime = GetNextDate();

            delayTime = (_nextRunTime - DateTime.UtcNow).TotalMilliseconds;

            return (int)delayTime;
        }

        private DateTime GetNextDate() => DateTime.UtcNow.AddSeconds(_configuration.DataSynchronizationTimeout);
    }
}
