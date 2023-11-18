namespace EntityGuardian.Utilities;

internal class CacheManager : ICacheManager
{
    private readonly IMemoryCache _memoryCache;
    public CacheManager(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public List<(string key, T data)> GetList<T>(string mainKey)
    {
        var coherentState = typeof(MemoryCache)
            .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

        var coherentStateValue = coherentState?.GetValue(_memoryCache);

        var cacheEntriesCollectionDefinition = coherentStateValue?
            .GetType()
            .GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);


        var cacheEntriesCollection = cacheEntriesCollectionDefinition?
            .GetValue(coherentStateValue) as ICollection;

        var cacheCollectionValues = new List<string>();

        if (cacheEntriesCollection != null)
        {
            foreach (var item in cacheEntriesCollection)
            {
                var methodInfo = item.GetType().GetProperty("Key");

                var val = methodInfo?.GetValue(item);

                cacheCollectionValues.Add(val?.ToString());
            }
        }

        var regex = new Regex(mainKey, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var keysToRemove = cacheCollectionValues
            .Where(d => regex.IsMatch(d))
            .Select(d => d)
            .ToList();

        List<(string key, T data)> list = new();

        foreach (var key in keysToRemove)
        {
            list.Add((key, _memoryCache.Get<T>(key)));
        }

        return list;
    }

    public void Add(string key, object data) => _memoryCache.Set(key, data);

    public bool IsExists(string key) => _memoryCache.TryGetValue(key, out _);

    public void Remove(string key) => _memoryCache.Remove(key);

}