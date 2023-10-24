using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Storages;
using EntityGuardian.Storages.SqlServer;
using EntityGuardian.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EntityGuardian.Extensions
{
    public static class StorageExtensions
    {
        public static void UseSqlServerStorage(
            this EntityGuardianOption options,
            IServiceCollection services)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            services.AddDbContext<EntityGuardianDbContext>();

            services.AddSingleton<IStorageService, SqlServerStorage>();

            ServiceTool.Create(services);

            var storage = ServiceTool.ServiceProvider.GetService<IStorageService>();
            storage.CreateDatabaseTablesAsync();
        }
    }
}
