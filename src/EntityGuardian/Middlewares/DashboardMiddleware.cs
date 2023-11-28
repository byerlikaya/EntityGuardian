namespace EntityGuardian.Middlewares;

public class DashboardMiddleware
{
    private IStorageService _storageService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly EntityGuardianOption _options;
    private readonly StaticFileMiddleware _staticFileMiddleware;

    public DashboardMiddleware(
        IWebHostEnvironment webHostEnvironment,
        ILoggerFactory loggerFactory,
        EntityGuardianOption options,
        RequestDelegate next
       )
    {
        _webHostEnvironment = webHostEnvironment;
        _loggerFactory = loggerFactory;
        _options = options;
        _staticFileMiddleware = CreateStaticFileMiddleware(next);
    }

    public async Task Invoke(HttpContext httpContext, IStorageService storageService)
    {
        _storageService = storageService;

        if (await TryHandleSpecialPath(httpContext))
            return;
        await _staticFileMiddleware.Invoke(httpContext);
    }

    private async Task<bool> TryHandleSpecialPath(HttpContext httpContext)
    {
        var httpRequest = httpContext.Request;
        var httpMethod = httpRequest.Method;

        if (httpMethod is not "GET")
            return false;

        var pageType = httpRequest.Path.GetPageType(_options.RoutePrefix) switch
        {
            PageType.None => RespondWithRedirect(httpContext),
            PageType.Index => await RespondWithIndexHtml(httpContext),
            PageType.Data => await RespondWithDataHtml(httpContext),
            PageType.ChangeWrapperDetail => await RespondWithChangeWrapperDetailHtml(httpContext),
            PageType.ChangeDetail => await RespondWithChangeDetailHtml(httpContext),
            _ => PageType.None
        };

        return pageType is not PageType.None;
    }

    private static void SetResponseContent(HttpContext httpContext, ResponseContentType responseContentType)
    {
        httpContext.Response.StatusCode = HttpStatusCode.OK.GetHashCode();
        httpContext.Response.ContentType = responseContentType == ResponseContentType.Html
            ? "text/html;charset=utf-8"
            : "application/json; charset=utf-8";
    }

    private static PageType RespondWithRedirect(HttpContext httpContext)
    {
        var httpRequest = httpContext.Request;
        var value = httpRequest.Path.Value;

        var relativeIndexUrl = string.IsNullOrWhiteSpace(value) || value.EndsWith('/')
            ? "index.html"
            : $"{value.Split('/').Last()}/index.html";

        httpContext.Response.StatusCode = HttpStatusCode.MovedPermanently.GetHashCode();
        httpContext.Response.Headers["Location"] = relativeIndexUrl;

        return PageType.None;
    }

    private async Task<PageType> RespondWithIndexHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Html);

        await using var stream = IndexStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);

        return PageType.Index;
    }

    private async Task<PageType> RespondWithChangeWrapperDetailHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Html);

        var guid = httpContext.Request.Query["guid"];

        await using var stream = ChangeWrapperDetailStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
        htmlBuilder.Replace("#guid", guid);

        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);

        return PageType.ChangeWrapperDetail;
    }

    private async Task<PageType> RespondWithChangeDetailHtml(HttpContext httpContext)
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

        return PageType.ChangeDetail;
    }

    private async Task<PageType> RespondWithDataHtml(HttpContext httpContext)
    {
        SetResponseContent(httpContext, ResponseContentType.Json);

        var type = httpContext.Request.Query["type"];

        await using var stream = DataStream();
        using var reader = new StreamReader(stream);
        var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());

        htmlBuilder.Replace("#entity-guardian-data", await GetJson(httpContext, type));

        await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);

        return PageType.Data;
    }

    private async Task<string> GetJson(HttpContext httpContext, StringValues type)
    {
        if (string.Equals(type, "changewrappers", StringComparison.OrdinalIgnoreCase))
            return await ChangeWrappersJson(httpContext);

        if (string.Equals(type, "changes", StringComparison.OrdinalIgnoreCase))
            return await ChangesJson(httpContext);

        return string.Empty;
    }

    private async Task<string> ChangeWrappersJson(HttpContext httpContext)
    {
        var changeWrappers = await _storageService.ChangeWrappersAsync(CreateChangeWrapperRequest(httpContext));

        SetChangeWrapperEntites(changeWrappers.ResultObject);

        return JsonSerializer.Serialize(changeWrappers, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static void SetChangeWrapperEntites(IEnumerable<ChangeWrapper> changeWrappers)
    {
        var entties = new List<string>();

        foreach (var changeWrapper in changeWrappers)
        {
            StringBuilder stringBuilder = new();
            foreach (var change in changeWrapper.Changes.Distinct())
            {
                if (entties.Exists(x => x == change.EntityName)) continue;
                if (string.IsNullOrWhiteSpace(change.EntityName)) continue;
                stringBuilder.AppendLine($"({change.TransactionType}) - {change.EntityName}<br>");
                entties.Add(change.EntityName);
            }
            changeWrapper.Entities = stringBuilder.ToString();
        }
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

        var contextId = Guid.TryParse(httpContext.Request.Query["contextId"], out var guid) ? guid : (Guid?)null;
        var entities = httpContext.Request.Query["entities"];
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
            TransactionCount = transactionCount,
            Username = username,
            IpAddress = ipAddress,
            ContextId = contextId,
            EntityName = entities
        };
    }

    private static ChangesRequest CreateChangeRequest(HttpContext httpContext)
    {
        var baseRequest = CreateBaseRequest(httpContext);

        var guid = Guid.Parse(httpContext.Request.Query["guid"]);

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