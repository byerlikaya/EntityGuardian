namespace EntityGuardian.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityGuardian(
        this IServiceCollection services,
        Action<EntityGuardianOption> configuration)
    {

        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddMemoryCache();

        services.AddDbContext<EntityGuardianDbContext>();

        services.AddSingleton<ICacheManager, CacheManager>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton(_ =>
        {
            var configurationInstance = new EntityGuardianOption();
            configuration(configurationInstance);
            return configurationInstance;
        });

        services.AddSingleton<IStorageService, SqlServerStorage>();

        services.AddTransient<EntityGuardianInterceptor>();

        services.AddHostedService<DataBackgroundService>();

        ServiceTool.Build(services);

        return services;
    }
}