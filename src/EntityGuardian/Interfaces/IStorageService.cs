using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        Task InstallAsync();

        Task CreateAsync();

        Task<List<T>> GetAsync<T>();
    }
}
