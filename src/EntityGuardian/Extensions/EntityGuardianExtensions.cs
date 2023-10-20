using EntityGuardian.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace EntityGuardian.Extensions
{
    public static class EntityGuardianExtensions
    {
        public static void UseEntityGuardian<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
            => app.UseMiddleware<DbContextMiddleware<TContext>>();
    }
}
