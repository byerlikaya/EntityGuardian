namespace EntityGuardian.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityGuardian(
        this IServiceCollection services,
        string connectionString,
        Action<EntityGuardianOption> configuration)
    {
        ArgumentNullControl(services, connectionString, configuration);

        SingletonServices(services, configuration);

        ScopedServices(services, connectionString);

        return services;
    }

    private static void ScopedServices(
        IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<EntityGuardianDbContext>(x => x.UseSqlServer(connectionString));

        services.AddScoped<IStorageService, SqlServerStorage>();

        services.AddScoped<EntityGuardianInterceptor>();
    }

    private static void SingletonServices(
        IServiceCollection services,
        Action<EntityGuardianOption> configuration)
    {
        services.AddMemoryCache();

        services.AddSingleton<ICacheManager, CacheManager>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton(_ =>
        {
            var configurationInstance = new EntityGuardianOption();
            configuration(configurationInstance);
            return configurationInstance;
        });

        services.AddHostedService<DataBackgroundService>();
    }

    private static void ArgumentNullControl(
        IServiceCollection services,
        string connectionString,
        Action<EntityGuardianOption> configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(connectionString);
    }
}