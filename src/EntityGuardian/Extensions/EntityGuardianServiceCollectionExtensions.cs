using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ServiceTool.Create(services);

            return services;
        }
    }
}
