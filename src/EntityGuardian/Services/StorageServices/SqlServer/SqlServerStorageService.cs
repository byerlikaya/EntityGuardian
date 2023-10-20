using Dapper;
using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace EntityGuardian.Services.StorageServices.SqlServer
{
    public class SqlServerStorageService : IStorageService
    {
        private ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        private readonly IDbConnection _dbConnection = ServiceTool.ServiceProvider.GetService<IDbConnection>();

        public void Install()
        {
            var script = GetSqlScript() ?? throw new ArgumentNullException("GetSqlScript()");
            _dbConnection.Execute(script);
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        private static string GetSqlScript()
            => GetStringResource(typeof(SqlServerStorageService).GetTypeInfo().Assembly, "EntityGuardian.Storages.SqlServer.Install.sql");

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
