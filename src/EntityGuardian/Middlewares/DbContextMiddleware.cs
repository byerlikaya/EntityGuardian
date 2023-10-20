using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EntityGuardian.Middlewares
{
    public class DbContextMiddleware<TContext>
        where TContext : DbContext
    {
        private readonly RequestDelegate _next;

        public DbContextMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext httpContext, TContext context)
        {
            httpContext.Items["DbContext"] = context;
            await _next(httpContext);
        }
    }
}
