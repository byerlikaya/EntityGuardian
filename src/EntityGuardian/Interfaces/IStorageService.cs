using EntityGuardian.Entities;
using EntityGuardian.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void CreateDatabaseTables();

        Task Synchronization();

        Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest);

        Task<ResponseData<IEnumerable<Change>>> ChangesAsync(ChangesRequest searchRequest);

        Task<Change> ChangeAsync(Guid guid);
    }
}
