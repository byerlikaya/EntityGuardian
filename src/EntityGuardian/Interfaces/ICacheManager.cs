namespace EntityGuardian.Interfaces
{
    public interface ICacheManager
    {
        T Get<T>(string key);

        object Get(string key);

        void Add(string key, object data);

        bool IsExists(string key);

        void Remove(string key);
    }
}
