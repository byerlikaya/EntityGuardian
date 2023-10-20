using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityGuardian.BackgroundServices
{
    public class DataBackgroundService : BackgroundService
    {
        private const string CronExpression = "0/5 * * * * ?"; // 5 seconds
        private DateTime _nextRunTime = DateTime.UtcNow;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelayTime(), cancellationToken);
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
            var cron = Cronos.CronExpression.Parse(CronExpression, CronFormat.IncludeSeconds);
            var next = cron.GetNextOccurrence(DateTime.UtcNow);
            return next ?? DateTime.UtcNow;
        }

    }
}
