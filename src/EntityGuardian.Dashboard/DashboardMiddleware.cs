using EntityGuardian.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NETSTANDARD2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace EntityGuardian.Dashboard
{
    public class DashboardMiddleware
    {
        private const string EmbeddedFileNamespace = "EntityGuardian.Dashboard.wwwroot";
        private readonly StaticFileMiddleware _staticFileMiddleware;

        public DashboardMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            IStorageService storageService)
        {

            _staticFileMiddleware = DashboardMiddleware.CreateStaticFileMiddleware(next, hostingEnv, loggerFactory);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            //// If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            //if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$",  RegexOptions.IgnoreCase))
            //{
            //    // Use relative redirect to support proxy environments
            //    var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
            //        ? "index.html"
            //        : $"{path.Split('/').Last()}/index.html";

            //    RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            //    return;
            //}

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape("")}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            using (var stream = IndexStream())
            {
                using (var reader = new StreamReader(stream))
                {

                    var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());

                    StringBuilder stringBuilder = new();

                    for (int i = 0; i < 100; i++)
                    {
                        stringBuilder.Append("<tr>\r\n    <td>Tiger Nixon</td>\r\n    <td>System Architect</td>\r\n    <td>Edinburgh</td>\r\n    <td>61</td>\r\n    <td>2011-04-25</td>\r\n    <td>$320,800</td>\r\n</tr>");
                    }

                    htmlBuilder.Replace("#entity-guardian-data", stringBuilder.ToString());


                    await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        public Func<Stream> IndexStream { get; set; } = () => typeof(DashboardMiddleware).GetTypeInfo().Assembly
            .GetManifestResourceStream("EntityGuardian.Dashboard.wwwroot.index.html");

        private static StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = "",
                FileProvider = new EmbeddedFileProvider(typeof(DashboardMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
            };

            return new StaticFileMiddleware(next, hostingEnv, Microsoft.Extensions.Options.Options.Create(staticFileOptions), loggerFactory);
        }
    }
}
