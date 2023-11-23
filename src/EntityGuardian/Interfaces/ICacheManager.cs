namespace EntityGuardian.Interfaces;

public interface ICacheManager
{
    List<(string key, T data)> GetList<T>(string mainKey);

    void AddCache(string key, object data);

    bool IsExists(string key);

    void Remove(string key);
}