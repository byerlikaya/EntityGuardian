using EntityGuardian.Interfaces;
using EntityGuardian.Middlewares;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityGuardian.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEntityGuardian<TContext>(
            this IApplicationBuilder app)
            where TContext : DbContext
        {
            var storage = ServiceTool.ServiceProvider.GetService<IStorageService>();
            storage.CreateDatabaseTables(true);

            app.UseMiddleware<DbContextMiddleware<TContext>>();

            app.UseMiddleware<DashboardMiddleware>();

            return app;
        }
    }
}
