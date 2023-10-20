using EntityGuardian.Interfaces;
using EntityGuardian.Options;
using EntityGuardian.Storages.SqlServer;
using EntityGuardian.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;

namespace EntityGuardian.Extensions
{
    public static class StorageExtensions
    {
        public static void UseSqlServerStorage(
            this EntityGuardianOption options,
            IServiceCollection services,
            string connectionString)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

            services.AddSingleton<IStorageService, SqlServerStorage>();

            ServiceTool.Create(services);

            var storage = ServiceTool.ServiceProvider.GetService<IStorageService>();
            storage.Install();
        }
    }
}
