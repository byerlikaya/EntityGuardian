using DataAuditing.Interfaces;
using DataAuditing.Options;
using DataAuditing.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace EntityGuardian.Extensions
{
    public static class DataAuditingServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAuditing(
            this IServiceCollection services,
            Action<IDataAuditingConfiguration> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<IDataAuditingConfiguration>(config =>
            {
                var configurationInstance = new DataAuditingConfiguration();

                configuration(configurationInstance);

                return configurationInstance;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ServiceTool.Create(services);

            return services;
        }
    }
}
