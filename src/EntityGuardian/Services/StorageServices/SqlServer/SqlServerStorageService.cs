using Dapper;
using EntityGuardian.Entities;
using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EntityGuardian.Services.StorageServices.SqlServer
{
    public class SqlServerStorageService : IStorageService
    {
        private readonly ICacheManager _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        private readonly IDbConnection _dbConnection = ServiceTool.ServiceProvider.GetService<IDbConnection>();

        public void Install()
            => _dbConnection.Execute(GetSqlScript());

        public async Task CreateAsync()
        {
            var memoryData = _cacheManager.GetList<ChangeWrapper>(nameof(ChangeWrapper));

            if (memoryData.Any())
            {
                if (_dbConnection.State == ConnectionState.Closed)
                    _dbConnection.Open();

                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        var changeWrappers = memoryData
                            .Select(x => x.data)
                            .ToList();

                        await _dbConnection.ExecuteAsync("""
                                                         INSERT INTO [dbo].[ChangeWrapper]
                                                                    ([Guid]
                                                                    ,[UserName]
                                                                    ,[IpAddress]
                                                                    ,[TargetName]
                                                                    ,[MethodName])
                                                              VALUES
                                                                    (@Guid,
                                                         			@UserName,
                                                         			@IpAddress,
                                                         		    @TargetName,
                                                         		    @MethodName)
                                                         """, changeWrappers, transaction);

                        var changes = changeWrappers
                            .SelectMany(x => x.Changes)
                            .ToList();

                        if (changes.Any())
                        {
                            await _dbConnection.ExecuteAsync("""
                                                             INSERT INTO [dbo].[Change]
                                                                        ([Guid]
                                                                        ,[ChangeWrapperGuid]
                                                                        ,[ActionType]
                                                                        ,[EntityName]
                                                                        ,[OldData]
                                                                        ,[NewData]
                                                                        ,[ModifiedDate])
                                                                  VALUES
                                                                        (@Guid,
                                                             		    @ChangeWrapperGuid,
                                                             			@ActionType,
                                                             			@EntityName,
                                                             			@OldData,
                                                             			@NewData,
                                                             			@ModifiedDate)
                                                             """, changes, transaction);
                        }

                        transaction.Commit();

                        memoryData.ForEach(x => _cacheManager.Remove(x.key));
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        private static string GetSqlScript()
            => GetStringResource(typeof(SqlServerStorageService).GetTypeInfo().Assembly, "EntityGuardian.Services.StorageServices.SqlServer.Install.sql");

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
