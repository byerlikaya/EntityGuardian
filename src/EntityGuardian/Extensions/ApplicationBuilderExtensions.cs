namespace EntityGuardian.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseEntityGuardian(this IApplicationBuilder app) =>
        app.UseMiddleware<DashboardMiddleware>();
}