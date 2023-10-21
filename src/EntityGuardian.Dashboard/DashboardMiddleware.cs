using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityGuardian.Dashboard
{
    public class DashboardMiddleware
    {
        private RequestDelegate _next;

        public DashboardMiddleware(RequestDelegate next)
        {
            _next = next;
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
            }

            //await _staticFileMiddleware.Invoke(httpContext);
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            using (var stream = IndexStream())
            {
                using var reader = new StreamReader(stream);

                // Inject arguments before writing to response
                var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
                //foreach (var entry in GetIndexArguments())
                //{
                //    htmlBuilder.Replace(entry.Key, entry.Value);
                //}

                await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }

        public Func<Stream> IndexStream { get; set; } = () => typeof(DashboardMiddleware).GetTypeInfo().Assembly
            .GetManifestResourceStream("EntityGuardian.Dashboard.index.html");
    }
}
