using EntityGuardian.BackgroundServices;
using EntityGuardian.Enums;
using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Storages;
using EntityGuardian.Storages.SqlServer;
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
        public static IServiceCollection AddEntityGuardian(
            this IServiceCollection services,
            Action<EntityGuardianOption> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<EntityGuardianOption>(_ =>
            {
                var configurationInstance = new EntityGuardianOption();

                configuration(configurationInstance);

                if (configurationInstance.StorageType == StorageType.SqlServer)
                {
                    services.AddSingleton<IStorageService, SqlServerStorage>();
                    ServiceTool.Create(services);
                }

                return configurationInstance;
            });

            services.AddMemoryCache();

            services.AddDbContext<EntityGuardianDbContext>();

            services.TryAddSingleton<ICacheManager, CacheManager>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IHostedService, DataBackgroundService>();

            ServiceTool.Create(services);

            return services;
        }
    }
}
