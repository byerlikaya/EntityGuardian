

using EntityGuardian.Middlewares;
using Microsoft.AspNetCore.Builder;

#if NETSTANDARD2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace EntityGuardian.Dashboard
{
    public static class DashboardBuilderExtensions
    {
        public static IApplicationBuilder UseEntityGuardianDashboard(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DashboardMiddleware>();
        }

        //public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
        //{
        //    //using (var scope = app.ApplicationServices.CreateScope())
        //    //{
        //    //    options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerUIOptions>>().Value;
        //    //    setupAction?.Invoke(options);
        //    //}

        //    // To simplify the common case, use a default that will work with the SwaggerMiddleware defaults
        //    //if (options.ConfigObject.Urls == null)
        //    //{
        //    //    var hostingEnv = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        //    //    options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{hostingEnv.ApplicationName} v1", Url = "v1/swagger.json" } };
        //    //}

        //    return app.UseSwaggerUI();
        //}
    }
}
