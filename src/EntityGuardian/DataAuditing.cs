using Castle.DynamicProxy;
using EntityGuardian.Entities;
using EntityGuardian.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityGuardian
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class DataAuditing : Attribute, IInterceptor
    {
        private ChangeWrapper _changeWrapper;
        private readonly DbContext _dbContext;
        private readonly string _ipAddress;

        public DataAuditing()
        {
            var httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

            _ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (httpContextAccessor.HttpContext.Items["DbContext"] is not DbContext dbContext)
                return;

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
                            ActionType = "INSERT",
                            NewData = JsonSerializer.Serialize(entityEntry.Entity),
                            OldData = string.Empty,
                            ModifiedDate = DateTime.Now,
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
                                ActionType = "UPDATE",
                                NewData = JsonSerializer.Serialize(currentEntity),
                                OldData = JsonSerializer.Serialize(dbEntity),
                                EntityName = entityEntry.Entity.ToString(),
                                ModifiedDate = DateTime.Now
                            });
                        }
                        break;
                    }
                    case EntityState.Deleted:
                    {
                        _changeWrapper.Changes.Add(new Change
                        {
                            ActionType = "DELETE",
                            NewData = JsonSerializer.Serialize(entityEntry.Entity),
                            OldData = string.Empty,
                            ModifiedDate = DateTime.Now,
                            EntityName = entityEntry.Entity.ToString()
                        });

                        break;
                    }
                }
            }
        }

        private static void DbContext_SavedChanges(object sender, SavedChangesEventArgs e)
        {

        }

    }
}
