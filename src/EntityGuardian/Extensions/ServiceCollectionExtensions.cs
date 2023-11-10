namespace EntityGuardian.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityGuardian(this IServiceCollection services,
        Action<EntityGuardianOption> configuration)
    {

        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddMemoryCache();

        services.AddDbContext<EntityGuardianDbContext>();

        services.TryAddSingleton<ICacheManager, CacheManager>();

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.TryAddSingleton<ProxyGenerator>();

        services.TryAddSingleton(_ =>
        {
            var configurationInstance = new EntityGuardianOption();

            configuration(configurationInstance);

            return configurationInstance;
        });

        services.TryAddSingleton<IStorageService, SqlServerStorage>();

        services.TryAddScoped<IInterceptor, EntityGuardianInterceptor>();

        services.AddHostedService<DataBackgroundService>();

        var assembly = Assembly.GetEntryAssembly();

        var serviceProvider = services.BuildServiceProvider();

        var count = services.Count;

        for (var i = 0; i < count; i++)
        {
            if (services[i].ServiceType.Assembly != assembly) continue;
            if (!services[i].ServiceType.IsInterface) continue;

            var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
            var actual = serviceProvider.GetRequiredService(services[i].ServiceType);
            var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
            var createdInterface = proxyGenerator.CreateInterfaceProxyWithTarget(services[i].ServiceType, actual, interceptors);
            var descriptor = new ServiceDescriptor(services[i].ServiceType, _ => createdInterface, ServiceLifetime.Scoped);
            services.Replace(descriptor);
        }

        return services;
    }
}