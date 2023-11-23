namespace EntityGuardian.Utilities;

internal class CacheManager : ICacheManager
{
    private readonly IMemoryCache _memoryCache;
    public CacheManager(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public List<(string key, T data)> GetList<T>(string mainKey) =>
        ValuesOfMatchingKeys(mainKey)
            .Select(key => (key, _memoryCache.Get<T>(key)))
            .ToList();

    public void AddCache(string key, object data) => _memoryCache.Set(key, data);

    public bool IsExists(string key) => _memoryCache.TryGetValue(key, out _);

    public void Remove(string key) => _memoryCache.Remove(key);

    private IEnumerable<string> ValuesOfMatchingKeys(string mainKey) =>
        ValuesOfKeys()
            .Where(d => RegexIsMatch(mainKey, d))
            .Select(d => d)
            .ToList();

    private IEnumerable<string> ValuesOfKeys() =>
        from object item in GetEntriesCollection()
        let methodInfo = item.GetType().GetProperty("Key")
        select methodInfo?.GetValue(item)
        into value
        select value?.ToString();

    private IEnumerable GetEntriesCollection()
    {
        var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

        var coherentStateValue = coherentState?.GetValue(_memoryCache);

        var cacheEntriesCollectionDefinition = coherentStateValue?
            .GetType()
            .GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

        return cacheEntriesCollectionDefinition?.GetValue(coherentStateValue) as IEnumerable ?? Enumerable.Empty<object>();
    }

    private static bool RegexIsMatch(string mainKey, string key) => new Regex(mainKey, GetRegexOptions()).IsMatch(key);

    private static RegexOptions GetRegexOptions() => RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase;
}