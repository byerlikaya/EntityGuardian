namespace EntityGuardian.Utilities;

internal static class ServiceTool
{
    public static IServiceProvider ServiceProvider { get; set; }
    public static void Build(IServiceCollection services) => ServiceProvider = services.BuildServiceProvider();
}