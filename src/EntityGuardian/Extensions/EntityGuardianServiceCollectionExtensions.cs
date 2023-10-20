using EntityGuardian.BackgroundServices;
using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Storages;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace EntityGuardian.Extensions
{
    public static class EntityGuardianServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityGuardian(
            this IServiceCollection services,
            Action<IEntityGuardianConfiguration> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<IEntityGuardianConfiguration>(config =>
            {
                var configurationInstance = new EntityGuardianConfiguration();

                configuration(configurationInstance);

                return configurationInstance;
            });

            services.AddMemoryCache();

            services.TryAddSingleton<ICacheManager, CacheManager>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddSingleton<IHostedService, DataBackgroundService>();

            ServiceTool.Create(services);

            return services;
        }
    }
}
