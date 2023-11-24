namespace EntityGuardian.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityGuardian(
        this IServiceCollection services,
        string connectionString,
        Action<EntityGuardianOption> configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(connectionString);

        services.AddMemoryCache();

        services.AddDbContext<EntityGuardianDbContext>(x => x.UseSqlServer(connectionString));

        services.AddSingleton<ICacheManager, CacheManager>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton(_ =>
        {
            var configurationInstance = new EntityGuardianOption();
            configuration(configurationInstance);
            return configurationInstance;
        });

        services.AddSingleton<IStorageService, SqlServerStorage>();

        services.AddScoped<EntityGuardianInterceptor>();

        services.AddHostedService<DataBackgroundService>();

        ServiceTool.Build(services);

        return services;
    }
}