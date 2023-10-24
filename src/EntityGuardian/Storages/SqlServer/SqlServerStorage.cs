using EntityGuardian.Entities;
using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        public async Task CreateDatabaseTablesAsync() => await _context.Database.ExecuteSqlRawAsync(GetSqlScript());

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

        public async Task<List<ChangeWrapper>> GetChangeWrappersAsync() =>
            await _context.ChangeWrapper
                .Include(x => x.Changes)
                .ToListAsync();

        private static string GetSqlScript()
            => GetStringResource(typeof(SqlServerStorage).GetTypeInfo().Assembly, "EntityGuardian.Storages.SqlServer.Install.sql");

        private static string GetStringResource(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
