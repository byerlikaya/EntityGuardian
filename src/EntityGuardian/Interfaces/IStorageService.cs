using EntityGuardian.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void CreateDatabaseTables();

        Task Synchronization();

        Task<List<ChangeWrapper>> GetChangeWrappersAsync();
    }
}
