using EntityGuardian.Entities;
using EntityGuardian.Entities.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void CreateDatabaseTables();

        Task Synchronization();

        Task<IDataResult<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(SearcRequest searchRequest);

        Task<IDataResult<IEnumerable<Change>>> ChangesAsync(SearcRequest searchRequest);

        Task<Change> ChangeAsync(Guid guid);
    }
}
