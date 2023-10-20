using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace EntityGuardian.Storages.SqlServer
{
    public class SqlServerInstaller : IStorage
    {
        private readonly DbConnection _dbConnection;

        public SqlServerInstaller(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }

        public void Install()
        {
            var script = GetSqlScript() ?? throw new ArgumentNullException("GetSqlScript()");

            _dbConnection.Execute(script);
        }

        private static string GetSqlScript()
            => GetStringResource(typeof(SqlServerInstaller).GetTypeInfo().Assembly, "DataAuditing.Storages.SqlServer.Install.sql");

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
