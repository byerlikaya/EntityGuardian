using Castle.DynamicProxy;
using EntityGuardian.Entities;
using EntityGuardian.Interfaces;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityGuardian
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly)]
    public class EntityGuardian : Attribute, IInterceptor
    {
        private ChangeWrapper _changeWrapper;
        private readonly DbContext _dbContext;
        private readonly string _ipAddress;
        private readonly ICacheManager _cacheManager;

        public EntityGuardian()
        {
            var httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

            _ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            if (httpContextAccessor.HttpContext?.Items["DbContext"] is not DbContext dbContext)
                return;

            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();

            dbContext.SavedChanges += DbContext_SavedChanges;
            dbContext.SavingChanges += DbContext_SavingChanges;
            dbContext.SaveChangesFailed += DbContext_SaveChangesFailed;

            _dbContext = dbContext;
        }

        public void Intercept(IInvocation invocation)
        {
            _changeWrapper = new ChangeWrapper
            {
                Guid = Guid.NewGuid(),
                TargetName = invocation.TargetType.FullName,
                MethodName = invocation.Method.Name,
                IpAddress = _ipAddress,
                TransactionDate = DateTime.UtcNow,
                Username = "Barış Yerlikaya",
                Changes = new List<Change>()
            };

            invocation.Proceed();
            var result = invocation.ReturnValue as Task;
            result?.Wait();
        }

        private static void DbContext_SaveChangesFailed(object sender, SaveChangesFailedEventArgs e)
        {

        }

        private void DbContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            var aa = Assembly.GetExecutingAssembly();

            var entityEntries = _dbContext.ChangeTracker
                .Entries()
                .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entityEntry in entityEntries)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                    {
                        _changeWrapper.Changes.Add(new Change
                        {
                            Guid = Guid.NewGuid(),
                            ChangeWrapperGuid = _changeWrapper.Guid,
                            ActionType = "INSERT",
                            NewData = JsonSerializer.Serialize(entityEntry.Entity),
                            OldData = string.Empty,
                            TransactionDate = DateTime.UtcNow,
                            EntityName = entityEntry.Entity.ToString()
                        });

                        break;
                    }
                    case EntityState.Modified:
                    {
                        var dbValues = entityEntry.GetDatabaseValues();

                        if (dbValues != null)
                        {
                            var dbEntity = dbValues.ToObject();

                            var currentValues = entityEntry.CurrentValues;

                            var currentEntity = currentValues.ToObject();

                            _changeWrapper.Changes.Add(new Change
                            {
                                Guid = Guid.NewGuid(),
                                ChangeWrapperGuid = _changeWrapper.Guid,
                                ActionType = "UPDATE",
                                NewData = JsonSerializer.Serialize(currentEntity),
                                OldData = JsonSerializer.Serialize(dbEntity),
                                EntityName = entityEntry.Entity.ToString(),
                                TransactionDate = DateTime.UtcNow
                            });
                        }
                        break;
                    }
                    case EntityState.Deleted:
                    {
                        _changeWrapper.Changes.Add(new Change
                        {
                            Guid = Guid.NewGuid(),
                            ChangeWrapperGuid = _changeWrapper.Guid,
                            ActionType = "DELETE",
                            NewData = string.Empty,
                            OldData = JsonSerializer.Serialize(entityEntry.Entity),
                            TransactionDate = DateTime.UtcNow,
                            EntityName = entityEntry.Entity.ToString()
                        });

                        break;
                    }
                }
            }

            var key = $"{nameof(ChangeWrapper)}_{_dbContext.ContextId}";

            if (_cacheManager.IsExists(key))
                key = $"{key}_{new Random().Next(0, 99999)}";

            _cacheManager.Add(key, _changeWrapper);

        }

        private static void DbContext_SavedChanges(object sender, SavedChangesEventArgs e)
        {

        }

    }
}
