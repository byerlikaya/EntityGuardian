namespace EntityGuardian.Extensions;

internal static class PathExtensions
{
    public static PageType GetPageType(this PathString pathString, string routePrefix)
    {
        if (pathString == null || pathString.Value is null)
            return PageType.None;

        if (pathString.IsRedirect(routePrefix))
            return PageType.Redirect;
        if (pathString.IsIndexHtml(routePrefix))
            return PageType.Index;
        if (pathString.IsDataHtml(routePrefix))
            return PageType.Data;
        if (pathString.IsChangeWrapperDetail(routePrefix))
            return PageType.ChangeWrapperDetail;
        return pathString.IsChangeDetail(routePrefix)
            ? PageType.ChangeDetail
            : PageType.None;
    }

    private static bool IsRedirect(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/?{Regex.Escape(routePrefix)}/?$", RegexOptions.IgnoreCase);

    private static bool IsIndexHtml(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?index.html$", RegexOptions.IgnoreCase);

    private static bool IsDataHtml(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?data.html$", RegexOptions.IgnoreCase);

    private static bool IsChangeWrapperDetail(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?change-wrapper-detail.html$", RegexOptions.IgnoreCase);

    private static bool IsChangeDetail(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?change-detail.html$", RegexOptions.IgnoreCase);
}