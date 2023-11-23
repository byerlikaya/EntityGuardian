namespace EntityGuardian.Middlewares;

public class DashboardMiddleware
{
    private readonly IStorageService _storageService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILoggerFactory _loggerFactory;

    private readonly EntityGuardianOption _options;
    private readonly StaticFileMiddleware _staticFileMiddleware;


    public DashboardMiddleware(
        IStorageService storageService,
        IWebHostEnvironment webHostEnvironment,
        ILoggerFactory loggerFactory,
        EntityGuardianOption options,
        RequestDelegate next
       )
    {
        _storageService = storageService;
        _webHostEnvironment = webHostEnvironment;
        _loggerFactory = loggerFactory;
        _options = options;
        _staticFileMiddleware = CreateStaticFileMiddleware(next);
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var httpRequest = httpContext.Request;
        var httpMethod = httpRequest.Method;
        var path = httpRequest.Path;

        if (httpMethod is "GET" && path.Value is not null)
            if (await TryHandleSpecialPath(httpContext, path))
                return;

        await _staticFileMiddleware.Invoke(httpContext);
    }

    private async Task<bool> TryHandleSpecialPath(HttpContext httpContext, PathString path)
    {
        if (path.IsRedirect(_options.RoutePrefix))
        {
            RespondWithRedirect(httpContext.Response, path);
        }
        else if (path.IsIndexHtml(_options.RoutePrefix))
        {
            await RespondWithIndexHtml(httpContext);
        }
        else if (path.IsDataHtml(_options.RoutePrefix))
        {
            await RespondWithDataHtml(httpContext);
        }
        else if (path.IsChangeWrapperDetail(_options.RoutePrefix))
        {
            await RespondWithChangeWrapperDetailHtml(httpContext);
        }
        else if (path.IsChangeDetail(_options.RoutePrefix))
        {
            await RespondWithChangeDetailHtml(httpContext);
        }
        else
        {
            return false;
        }

        return true;
    }

    private static void SetResponseContent(HttpContext httpContext, ResponseContentType responseContentType)
    {
        httpContext.Response.StatusCode = HttpStatusCode.OK.GetHashCode();
        httpContext.Response.ContentType = responseContentType == ResponseContentType.Html
            ? "text/html;charset=utf-8"
            : "application/json; charset=utf-8";
    }

    private static void RespondWithRedirect(HttpResponse response, PathString pathString)
    {
        var value = pathString.Value;

        var relativeIndexUrl = string.IsNullOrWhiteSpace(value) || value.EndsWith('/')
            ? "index.html"
            : $"{value.Split('/').Last()}/index.html";

        response.StatusCode = HttpStatusCode.MovedPermanently.GetHashCode();
        response.Headers["Location"] = relativeIndexUrl;
    }

    private async Task RespondWithIndexHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Html);

        await using var stream = IndexStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
    }

    private async Task RespondWithChangeWrapperDetailHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Html);

        var guid = httpContext.Request.Query["guid"];

        await using var stream = ChangeWrapperDetailStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
        htmlBuilder.Replace("#guid", guid);

        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
    }

    private async Task RespondWithChangeDetailHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Html);

        var guid = httpContext.Request.Query["guid"];
        var changeWrapperGuid = httpContext.Request.Query["change-wrapper-guid"];

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
        SetResponseContent(httpContext, ResponseContentType.Json);

        var type = httpContext.Request.Query["type"];

        await using var stream = DataStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());

        var json = "";

        if (string.Equals(type, "changewrappers", StringComparison.OrdinalIgnoreCase))
        {
            json = await ChangeWrappersJson(httpContext);
        }
        else if (string.Equals(type, "changes", StringComparison.OrdinalIgnoreCase))
        {
            json = await ChangesJson(httpContext);
        }

        htmlBuilder.Replace("#entity-guardian-data", json);

        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
    }

    private async Task<string> ChangeWrappersJson(HttpContext httpContext)
    {
        var changeWrappers = await _storageService.ChangeWrappersAsync(CreateChangeWrapperRequest(httpContext));

        return JsonSerializer.Serialize(changeWrappers, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private async Task<string> ChangesJson(HttpContext httpContext)
    {
        var changes = await _storageService.ChangesAsync(CreateChangeRequest(httpContext));

        return JsonSerializer.Serialize(changes, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static BaseRequest CreateBaseRequest(HttpContext httpContext)
    {
        var start = int.Parse(httpContext.Request.Query["start"]);
        var max = int.Parse(httpContext.Request.Query["length"]);
        var orderIndex = httpContext.Request.Query["order[0][column]"];
        var orderName = httpContext.Request.Query[$"columns[{orderIndex}][data]"];
        var orderType = httpContext.Request.Query["order[0][dir]"];

        return new BaseRequest
        {
            Start = start,
            Max = max,
            OrderBy = new Sorting
            {
                Name = orderName,
                OrderType = orderType
            }
        };
    }

    private static ChangeWrapperRequest CreateChangeWrapperRequest(HttpContext httpContext)
    {
        var baseRequest = CreateBaseRequest(httpContext);

        var mainEntity = httpContext.Request.Query["mainEntity"];
        var transactionCount = int.TryParse(httpContext.Request.Query["transactionCount"], out var count) ? count : (int?)null;
        var username = httpContext.Request.Query["username"];
        var ipAddress = httpContext.Request.Query["ipAddress"];

        return new ChangeWrapperRequest
        {
            Start = baseRequest.Start,
            Max = baseRequest.Max,
            OrderBy = new Sorting
            {
                Name = baseRequest.OrderBy.Name,
                OrderType = baseRequest.OrderBy.OrderType
            },
            MainEntity = mainEntity,
            IpAddress = ipAddress,
            TransactionCount = transactionCount,
            Username = username
        };
    }

    private static ChangesRequest CreateChangeRequest(HttpContext httpContext)
    {
        var baseRequest = CreateBaseRequest(httpContext);

        var guid = Guid.Parse(httpContext.Request.Query["guid"]);
        var rank = int.TryParse(httpContext.Request.Query["rank"], out var rankValue) ? rankValue : (int?)null;

        var transactionType = httpContext.Request.Query["transactionType"];
        var entityName = httpContext.Request.Query["entityName"];

        return new ChangesRequest
        {
            ChangeWrapperGuid = guid,
            Start = baseRequest.Start,
            Max = baseRequest.Max,
            OrderBy = new Sorting
            {
                Name = baseRequest.OrderBy.Name,
                OrderType = baseRequest.OrderBy.OrderType
            },
            EntityName = entityName,
            Rank = rank,
            TransactionType = transactionType
        };
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

    private StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(_options.RoutePrefix)
                ? string.Empty
                : $"/{_options.RoutePrefix}",
            FileProvider = new EmbeddedFileProvider(typeof(DashboardMiddleware).GetTypeInfo().Assembly, "EntityGuardian.Dashboard"),
        };

        return new StaticFileMiddleware(next, _webHostEnvironment, Microsoft.Extensions.Options.Options.Create(staticFileOptions), _loggerFactory);
    }
}