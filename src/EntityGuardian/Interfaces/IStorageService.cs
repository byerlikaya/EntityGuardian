using EntityGuardian.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        Task CreateDatabaseTablesAsync();

        Task Synchronization();

        Task<List<ChangeWrapper>> GetChangeWrappersAsync();
    }
}
