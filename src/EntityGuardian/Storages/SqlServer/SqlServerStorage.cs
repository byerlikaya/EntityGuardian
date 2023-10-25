using EntityGuardian.Entities;
using EntityGuardian.Entities.Results;
using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartOrderBy;
using SmartWhere;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace EntityGuardian.Storages.SqlServer
{
    internal class SqlServerStorage : IStorageService
    {
        private readonly ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();

        private readonly EntityGuardianDbContext _context = ServiceTool.ServiceProvider.GetService<EntityGuardianDbContext>();

        public void CreateDatabaseTables() => _context.Database.ExecuteSqlRaw(GetSqlScript());

        public async Task Synchronization()
        {
            var memoryData = _cacheManager.GetList<ChangeWrapper>(nameof(ChangeWrapper));

            if (memoryData.Any())
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var changeWrappers = memoryData
                        .Select(x => x.data)
                        .ToList();

                    await _context.ChangeWrapper.AddRangeAsync(changeWrappers);

                    await _context.SaveChangesAsync();

                    transaction.Complete();

                    memoryData.ForEach(x => _cacheManager.Remove(x.key));
                }
                catch
                {
                    transaction.Dispose();
                }
            }
        }

        public async Task<IDataResult<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(SearcRequest searchDto)
        {
            var query = _context.ChangeWrapper
                .Where(searchDto)
                .OrderBy(searchDto.OrderBy);

            var count = await query.CountAsync();

            var result = await query
                .Skip(searchDto.Start)
                .Take(searchDto.Max == default ? 10 : searchDto.Max)
                .ToListAsync();

            return new DataResult<List<ChangeWrapper>>(result, count);
        }

        public async Task<IDataResult<IEnumerable<Change>>> ChangesAsync(Guid guid)
        {
            var query = _context.Change
                .Where(x => x.ChangeWrapperGuid == guid);

            return new DataResult<IEnumerable<Change>>(await query.ToListAsync(), await query.CountAsync());
        }

        private static string GetSqlScript()
            => GetStringResource(typeof(SqlServerStorage).GetTypeInfo().Assembly, "EntityGuardian.Storages.SqlServer.Install.sql");

        private static string GetStringResource(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)
                               ?? throw new InvalidOperationException($"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
