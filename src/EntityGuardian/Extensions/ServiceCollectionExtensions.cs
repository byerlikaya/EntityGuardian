namespace EntityGuardian.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityGuardian(
        this IServiceCollection services,
        string connectionString,
        Action<EntityGuardianOption> options)
    {
        ArgumentNullControl(services, connectionString, options);

        SingletonServices(services, options);

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
        Action<EntityGuardianOption> options)
    {
        services.AddMemoryCache();

        services.AddSingleton<ICacheManager, CacheManager>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton(_ =>
        {
            var optionInstance = new EntityGuardianOption();
            options(optionInstance);
            return optionInstance;
        });

        services.AddHostedService<DataBackgroundService>();
    }

    private static void ArgumentNullControl(
        IServiceCollection services,
        string connectionString,
        Action<EntityGuardianOption> options)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(connectionString);
    }
}