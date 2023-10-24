using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NETSTANDARD2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace EntityGuardian.Middlewares
{
    public class DashboardMiddleware
    {
        private const string EmbeddedFileNamespace = "EntityGuardian.Dashboard";
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly IStorageService _storageService;

        public DashboardMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory)
        {
            _storageService = ServiceTool.ServiceProvider.GetService<IStorageService>();
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

                    var changeWrappers = await _storageService.GetChangeWrappersAsync();

                    var entityNames = changeWrappers
                        .OrderByDescending(x => x.Changes.FirstOrDefault()!.ModifiedDate)
                        .SelectMany(x => x.Changes)
                        .Select(x => x.EntityName)
                        .Distinct();

                    StringBuilder stringBuilder = new();

                    foreach (var entity in entityNames)
                    {
                        var change = changeWrappers
                            .OrderByDescending(x => x.Changes.FirstOrDefault()!.ModifiedDate)
                            .SelectMany(x => x.Changes)
                            .FirstOrDefault(x => string.Equals(x.EntityName, entity));

                        stringBuilder.AppendLine("<tr>");
                        stringBuilder.AppendLine($"<td>{entity}</td>");
                        stringBuilder.AppendLine($"<td>{change!.ActionType}</td>");
                        stringBuilder.AppendLine($"<td>{change!.ModifiedDate}</td>");
                        stringBuilder.AppendLine("<td></td>");
                        stringBuilder.AppendLine("</tr>");
                    }

                    htmlBuilder.Replace("#entity-guardian-data", stringBuilder.ToString());


                    await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        public Func<Stream> IndexStream { get; set; } = () => typeof(DashboardMiddleware).GetTypeInfo().Assembly
            .GetManifestResourceStream("EntityGuardian.Dashboard.index.html");

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
