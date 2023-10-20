using EntityGuardian.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EntityGuardian.Services.StorageServices
{
    internal class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;
        public CacheManager(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public void Add(string key, object data) => _memoryCache.Set(key, data);

        public T Get<T>(string key) => _memoryCache.Get<T>(key);

        public object Get(string key) => _memoryCache.Get(key);

        public bool IsExists(string key) => _memoryCache.TryGetValue(key, out _);

        public void Remove(string key) => _memoryCache.Remove(key);

    }
}
