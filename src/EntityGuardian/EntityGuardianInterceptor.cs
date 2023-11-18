using EntityGuardian.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityGuardian;

public class EntityGuardianInterceptor : SaveChangesInterceptor
{
    private readonly ICacheManager _cacheManager;
    private readonly ChangeWrapper _changeWrapper;

    public EntityGuardianInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ICacheManager cacheManager)
    {
        var httpContextAccessor1 = httpContextAccessor
                                   ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cacheManager = cacheManager
                        ?? throw new ArgumentNullException(nameof(cacheManager));

        var user = httpContextAccessor1.HttpContext?.User;
        var connection = httpContextAccessor1.HttpContext?.Connection;

        _changeWrapper = new ChangeWrapper
        {
            Guid = Guid.NewGuid(),
            //TargetName = invocation.TargetType.FullName,
            //MethodName = invocation.Method.Name,
            IpAddress = connection?.RemoteIpAddress?.ToString(),
            TransactionDate = DateTime.UtcNow,
            Username = user?.Identity?.Name ?? "undefined",
            Changes = new List<Change>()
        };
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ApplyChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyChanges(DbContext dbContext)
    {
        if (dbContext is null)
            return;

        var index = 1;

        foreach (var entityEntry in dbContext.BringTheEntriesToBeAffected())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    AddedChanges(index, entityEntry);
                    break;
                case EntityState.Modified:
                    ModifiedChanges(index, entityEntry);
                    break;
                case EntityState.Deleted:
                    DeletedChanges(index, entityEntry);
                    break;
            }
            index++;
        }

        AddCache(dbContext.ContextId.InstanceId);
    }

    private void AddCache(Guid contextId)
    {
        var key = $"{nameof(ChangeWrapper)}_{contextId}";

        if (_cacheManager.IsExists(key))
            key = $"{key}_{new Random().Next(0, 99999)}";

        _cacheManager.Add(key, _changeWrapper);
    }

    private void AddedChanges(int index, EntityEntry entityEntry)
    {
        _changeWrapper.Changes.Add(new Change
        {
            Guid = Guid.NewGuid(),
            ChangeWrapperGuid = _changeWrapper.Guid,
            TransactionType = TransactionType.Insert.ToString(),
            NewData = JsonSerializer.Serialize(entityEntry.Entity),
            OldData = string.Empty,
            EntityName = entityEntry.Entity.ToString(),
            TransactionDate = DateTime.UtcNow,
            Order = index
        });
    }

    private void ModifiedChanges(int index, EntityEntry entityEntry)
    {
        var dbValues = entityEntry.GetDatabaseValues();

        if (dbValues == null)
            return;

        var dbEntity = dbValues.ToObject();
        var currentValues = entityEntry.CurrentValues;
        var currentEntity = currentValues.ToObject();

        _changeWrapper.Changes.Add(new Change
        {
            Guid = Guid.NewGuid(),
            ChangeWrapperGuid = _changeWrapper.Guid,
            TransactionType = TransactionType.Update.ToString(),
            NewData = JsonSerializer.Serialize(currentEntity),
            OldData = JsonSerializer.Serialize(dbEntity),
            EntityName = entityEntry.Entity.ToString(),
            TransactionDate = DateTime.UtcNow,
            Order = index
        });
    }

    private void DeletedChanges(int index, EntityEntry entityEntry)
    {
        _changeWrapper.Changes.Add(new Change
        {
            Guid = Guid.NewGuid(),
            ChangeWrapperGuid = _changeWrapper.Guid,
            TransactionType = TransactionType.Delete.ToString(),
            NewData = string.Empty,
            OldData = JsonSerializer.Serialize(entityEntry.Entity),
            TransactionDate = DateTime.UtcNow,
            EntityName = entityEntry.Entity.ToString(),
            Order = index
        });
    }
}