namespace EntityGuardian.Extensions;

internal static class PathExtensions
{
    public static bool IsRedirect(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/?{Regex.Escape(routePrefix)}/?$", RegexOptions.IgnoreCase);

    public static bool IsIndexHtml(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?index.html$", RegexOptions.IgnoreCase);

    public static bool IsDataHtml(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?data.html$", RegexOptions.IgnoreCase);

    public static bool IsChangeWrapperDetail(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?change-wrapper-detail.html$", RegexOptions.IgnoreCase);

    public static bool IsChangeDetail(this PathString pathString, string routePrefix) =>
        pathString.Value is not null
        && Regex.IsMatch(pathString.Value, $"^/{Regex.Escape(routePrefix)}/?change-detail.html$", RegexOptions.IgnoreCase);
}