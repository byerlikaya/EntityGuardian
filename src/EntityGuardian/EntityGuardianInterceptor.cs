namespace EntityGuardian;

public class EntityGuardianInterceptor(
    IHttpContextAccessor httpContextAccessor,
    ICacheManager cacheManager) : SaveChangesInterceptor
{
    private readonly ICacheManager _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private ChangeWrapper _changeWrapper;

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
        var user = _httpContextAccessor.HttpContext?.User;
        var connection = _httpContextAccessor.HttpContext?.Connection;

        _changeWrapper = new ChangeWrapper
        {
            Guid = Guid.NewGuid(),
            IpAddress = connection?.RemoteIpAddress?.ToString(),
            TransactionDate = DateTime.UtcNow,
            Username = user?.Identity?.Name ?? "undefined",
            Changes = new List<Change>()
        };

        foreach (var entityEntry in dbContext.BringTheEntriesToBeAffected())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    AddedChanges(dbContext.ContextId.InstanceId, entityEntry);
                    break;
                case EntityState.Modified:
                    ModifiedChanges(dbContext.ContextId.InstanceId, entityEntry);
                    break;
                case EntityState.Deleted:
                    DeletedChanges(dbContext.ContextId.InstanceId, entityEntry);
                    break;
            }
        }

        AddCache(dbContext.ContextId.InstanceId);
    }

    private void AddCache(Guid contextId)
    {
        var key = $"{nameof(ChangeWrapper)}_{contextId}";

        if (_cacheManager.IsExists(key))
        {
#if NET6_0_OR_GREATER
            key = $"{key}_{Random.Shared.Next(0, 99999)}";
#else
            key = $"{key}_{new Random().Next(0, 99999)}";
#endif
        }

        _changeWrapper.DbContextId = contextId;
        _changeWrapper.TransactionCount = _changeWrapper.Changes.Count;

        _cacheManager.AddCache(key, _changeWrapper);
    }

    private Change GetChange(Guid dbContextId, string transactionType, string entityName) =>
        new()
        {
            Guid = Guid.NewGuid(),
            ChangeWrapperGuid = _changeWrapper.Guid,
            TransactionType = transactionType,
            EntityName = entityName,
            TransactionDate = DateTime.UtcNow,
            DbContextId = dbContextId
        };

    private void AddedChanges(Guid dbContextId, EntityEntry entityEntry)
    {
        var change = GetChange(dbContextId, nameof(TransactionType.Insert), entityEntry.Entity.ToString());

        change.NewData = JsonSerializer.Serialize(entityEntry.Entity);
        change.OldData = string.Empty;

        _changeWrapper.Changes.Add(change);
    }

    private void ModifiedChanges(Guid dbContextId, EntityEntry entityEntry)
    {
        var dbValues = entityEntry.GetDatabaseValues();

        if (dbValues is null)
            return;

        var dbEntity = dbValues.ToObject();
        var currentValues = entityEntry.CurrentValues;
        var currentEntity = currentValues.ToObject();

        var change = GetChange(dbContextId, nameof(TransactionType.Update), entityEntry.Entity.ToString());

        change.NewData = JsonSerializer.Serialize(currentEntity);
        change.OldData = JsonSerializer.Serialize(dbEntity);

        _changeWrapper.Changes.Add(change);
    }

    private void DeletedChanges(Guid dbContextId, EntityEntry entityEntry)
    {
        var change = GetChange(dbContextId, nameof(TransactionType.Delete), entityEntry.Entity.ToString());

        change.NewData = string.Empty;
        change.OldData = JsonSerializer.Serialize(entityEntry.Entity);

        _changeWrapper.Changes.Add(change);
    }

}