using System.Collections.Generic;

namespace EntityGuardian.Interfaces
{
    internal interface ICacheManager
    {
        List<(string key, T data)> GetList<T>(string mainKey);

        void Add(string key, object data);

        bool IsExists(string key);

        void Remove(string key);
    }
}
