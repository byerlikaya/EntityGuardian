namespace EntityGuardian.Middlewares
{
    public class DashboardMiddleware
    {
        private readonly EntityGuardianOption _options;
        private const string EmbeddedFileNamespace = "EntityGuardian.Dashboard";
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly IStorageService _storageService;

        public DashboardMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            EntityGuardianOption options,
            IStorageService storageService)
        {
            _options = options;
            _storageService = storageService;
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;


            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                RespondWithRedirect(httpContext.Response, relativeIndexUrl);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?data.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithDataHtml(httpContext);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?change-wrapper-detail.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithChangeWrapperDetailHtml(httpContext);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?change-detail.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithChangeDetailHtml(httpContext);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private static void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithChangeWrapperDetailHtml(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            var guid = httpContext.Request.Query["guid"].ToString();
            await using var stream = ChangeWrapperDetailStream();
            using var reader = new StreamReader(stream);
            var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
            htmlBuilder.Replace("#guid", guid);

            await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }

        private async Task RespondWithChangeDetailHtml(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            var guid = httpContext.Request.Query["guid"].ToString();
            var changeWrapperGuid = httpContext.Request.Query["change-wrapper-guid"].ToString();
            await using var stream = ChangeDetailStream();
            using var reader = new StreamReader(stream);
            var change = await _storageService.ChangeAsync(Guid.Parse(guid));
            var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());

            htmlBuilder.Replace("#old-data", change.OldData);
            htmlBuilder.Replace("#new-data", change.NewData);
            htmlBuilder.Replace("#changeWrapeerGuid", changeWrapperGuid);

            await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }

        private async Task RespondWithDataHtml(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json; charset=utf-8";

            var type = httpContext.Request.Query["type"].ToString();

            using (var stream = DataStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());

                    var start = int.Parse(httpContext.Request.Query["start"]);
                    var max = int.Parse(httpContext.Request.Query["length"]);
                    var orderIndex = httpContext.Request.Query["order[0][column]"].ToString();
                    var orderName = httpContext.Request.Query[$"columns[{orderIndex}][data]"].ToString();
                    var orderType = httpContext.Request.Query["order[0][dir]"].ToString();

                    if (string.Equals(type, "changewrappers", StringComparison.OrdinalIgnoreCase))
                    {
                        var targetName = httpContext.Request.Query["targetName"].ToString();
                        var methodName = httpContext.Request.Query["methodName"].ToString();
                        var username = httpContext.Request.Query["username"].ToString();
                        var ipAddress = httpContext.Request.Query["ipAddress"].ToString();

                        var changeWrappers = await _storageService.ChangeWrappersAsync(new ChangeWrapperRequest
                        {
                            Start = start,
                            Max = max,
                            OrderBy = new Sorting
                            {
                                Name = orderName,
                                OrderType = orderType
                            },
                            MethodName = methodName,
                            IpAddress = ipAddress,
                            TargetName = targetName,
                            Username = username
                        });

                        var json = JsonSerializer.Serialize(changeWrappers, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        htmlBuilder.Replace("#entity-guardian-data", json);
                    }
                    else if (string.Equals(type, "changes", StringComparison.OrdinalIgnoreCase))
                    {
                        var guid = Guid.Parse(httpContext.Request.Query["guid"]);
                        var order = int.TryParse(httpContext.Request.Query["order"], out var o) ? o : (int?)null;
                        var transactionType = httpContext.Request.Query["transactionType"].ToString();
                        var entityName = httpContext.Request.Query["entityName"].ToString();

                        var changes = await _storageService.ChangesAsync(new ChangesRequest
                        {
                            ChangeWrapperGuid = guid,
                            Start = start,
                            Max = max,
                            OrderBy = new Sorting
                            {
                                Name = orderName,
                                OrderType = orderType
                            },
                            EntityName = entityName,
                            Order = order,
                            TransactionType = transactionType
                        });

                        var json = JsonSerializer.Serialize(changes, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        htmlBuilder.Replace("#entity-guardian-data", json);
                    }


                    await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            await using var stream = IndexStream();
            using var reader = new StreamReader(stream);
            var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }

        private Func<Stream> IndexStream { get; } = ()
            => typeof(DashboardMiddleware)
                .GetTypeInfo()
                .Assembly
                .GetManifestResourceStream("EntityGuardian.Dashboard.html.index.html");

        private Func<Stream> DataStream { get; } = ()
            => typeof(DashboardMiddleware)
                .GetTypeInfo()
                .Assembly
                .GetManifestResourceStream("EntityGuardian.Dashboard.html.data.html");

        private Func<Stream> ChangeWrapperDetailStream { get; } = ()
            => typeof(DashboardMiddleware)
                .GetTypeInfo()
                .Assembly
                .GetManifestResourceStream("EntityGuardian.Dashboard.html.change-wrapper-detail.html");

        private Func<Stream> ChangeDetailStream { get; } = ()
            => typeof(DashboardMiddleware)
                .GetTypeInfo()
                .Assembly
                .GetManifestResourceStream("EntityGuardian.Dashboard.html.change-detail.html");

        private static StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory, EntityGuardianOption options)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix)
                    ? string.Empty
                    : $"/{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(DashboardMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
            };

            return new StaticFileMiddleware(next, hostingEnv, Microsoft.Extensions.Options.Options.Create(staticFileOptions), loggerFactory);
        }
    }
}
