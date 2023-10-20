using Cronos;
using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityGuardian.Services.BackgroundServices
{
    public class DataBackgroundService : BackgroundService
    {
        private DateTime _nextRunTime = DateTime.UtcNow;
        private readonly IStorageService _storageService = ServiceTool.ServiceProvider.GetService<IStorageService>();
        private static readonly EntityGuardianConfiguration Configuration = ServiceTool.ServiceProvider.GetService<EntityGuardianConfiguration>();

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelayTime(), cancellationToken);
                _storageService.Create();
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

        private static DateTime GetNextDate()
        {
            var cron = CronExpression.Parse(Configuration.CronExpression, CronFormat.IncludeSeconds);
            var next = cron.GetNextOccurrence(DateTime.UtcNow);
            return next ?? DateTime.UtcNow;
        }

    }
}
