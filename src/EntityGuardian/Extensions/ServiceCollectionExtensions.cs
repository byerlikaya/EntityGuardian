using EntityGuardian.BackgroundServices;
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
        public static void AddEntityGuardian(this IServiceCollection services,
            Action<EntityGuardianOption> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.TryAddSingleton(_ =>
            {
                var configurationInstance = new EntityGuardianOption();

                configuration(configurationInstance);

                return configurationInstance;
            });

            services.AddDbContext<EntityGuardianDbContext>();

            services.AddMemoryCache();

            services.TryAddSingleton<ICacheManager, CacheManager>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddSingleton<IStorageService, SqlServerStorage>();

            services.AddSingleton<IHostedService, DataBackgroundService>();

            ServiceTool.Create(services);
        }
    }
}
