using EntityGuardian.Entities;
using EntityGuardian.Entities.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void CreateDatabaseTables();

        Task Synchronization();

        Task<IDataResult<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(SearcRequest searchDto);
    }
}
