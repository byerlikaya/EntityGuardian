using EntityGuardian.Entities;
using EntityGuardian.Entities.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void CreateDatabaseTables(bool clearDataOnStartup);

        Task Synchronization();

        Task<IDataResult<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(SearcRequest searchDto);

        Task<IDataResult<IEnumerable<Change>>> ChangesAsync(Guid changeWrapperGuid);

        Task<Change> ChangeAsync(Guid guid);
    }
}
