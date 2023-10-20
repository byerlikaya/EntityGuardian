using EntityGuardian.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace EntityGuardian.Extensions
{
    public static class DataAuditingExtensions
    {
        public static void UseDataAuditing<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
            => app.UseMiddleware<DbContextMiddleware<TContext>>();
    }
}
