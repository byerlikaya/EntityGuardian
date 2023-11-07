namespace EntityGuardian.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseEntityGuardian<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
        {
            app.UseMiddleware<DbContextMiddleware<TContext>>();

            app.UseMiddleware<DashboardMiddleware>();
        }
    }
}
