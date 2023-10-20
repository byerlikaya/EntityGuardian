using EntityGuardian.BackgroundServices;
using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace EntityGuardian.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEntityGuardian(this IServiceCollection services,
            Action<EntityGuardianOption> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<EntityGuardianOption>(_ =>
            {
                var configurationInstance = new EntityGuardianOption();

                configuration(configurationInstance);

                return configurationInstance;
            });

            services.AddMemoryCache();

            services.TryAddSingleton<ICacheManager, CacheManager>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddSingleton<IHostedService, DataBackgroundService>();

            ServiceTool.Create(services);
        }
    }
}
