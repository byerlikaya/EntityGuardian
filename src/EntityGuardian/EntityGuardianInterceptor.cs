namespace EntityGuardian;

public class EntityGuardianInterceptor : SaveChangesInterceptor
{
    private readonly ICacheManager _cacheManager;
    private readonly ChangeWrapper _changeWrapper;

    public EntityGuardianInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ICacheManager cacheManager)
    {
        _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));

        if (httpContextAccessor is null)
            throw new ArgumentNullException(nameof(httpContextAccessor));

        var user = httpContextAccessor.HttpContext?.User;
        var connection = httpContextAccessor.HttpContext?.Connection;

        _changeWrapper = new ChangeWrapper
        {
            Guid = Guid.NewGuid(),
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
        var rank = 0;

        foreach (var entityEntry in dbContext.BringTheEntriesToBeAffected())
        {
            rank++;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    AddedChanges(rank, entityEntry);
                    break;
                case EntityState.Modified:
                    ModifiedChanges(rank, entityEntry);
                    break;
                case EntityState.Deleted:
                    DeletedChanges(rank, entityEntry);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    SetMainEntity(rank, entityEntry);
                    break;
            }
        }

        AddCache(rank, dbContext.ContextId.InstanceId);
    }

    private void AddCache(int rank, Guid contextId)
    {
        if (rank is 0)
            return;

        var key = $"{nameof(ChangeWrapper)}_{contextId}";

        if (_cacheManager.IsExists(key))
        {
#if NET6_0_OR_GREATER
            key = $"{key}_{Random.Shared.Next(0, 99999)}";
#else
            key = $"{key}_{new Random().Next(0, 99999)}";
#endif
        }

        _changeWrapper.TransactionCount = rank;

        _cacheManager.AddCache(key, _changeWrapper);
    }

    private Change GetChange(string transactionType, int rank, string entityName) =>
        new()
        {
            Guid = Guid.NewGuid(),
            ChangeWrapperGuid = _changeWrapper.Guid,
            TransactionType = transactionType,
            EntityName = entityName,
            TransactionDate = DateTime.UtcNow,
            Rank = rank
        };

    private void AddedChanges(int rank, EntityEntry entityEntry)
    {
        SetMainEntity(rank, entityEntry);

        var change = GetChange(nameof(TransactionType.Insert), rank, entityEntry.Entity.ToString());

        change.NewData = JsonSerializer.Serialize(entityEntry.Entity);
        change.OldData = string.Empty;

        _changeWrapper.Changes.Add(change);
    }

    private void ModifiedChanges(int rank, EntityEntry entityEntry)
    {
        SetMainEntity(rank, entityEntry);

        var dbValues = entityEntry.GetDatabaseValues();

        if (dbValues is null)
            return;

        var dbEntity = dbValues.ToObject();
        var currentValues = entityEntry.CurrentValues;
        var currentEntity = currentValues.ToObject();

        var change = GetChange(nameof(TransactionType.Update), rank, entityEntry.Entity.ToString());

        change.NewData = JsonSerializer.Serialize(currentEntity);
        change.OldData = JsonSerializer.Serialize(dbEntity);

        _changeWrapper.Changes.Add(change);
    }

    private void DeletedChanges(int rank, EntityEntry entityEntry)
    {
        SetMainEntity(rank, entityEntry);

        var change = GetChange(nameof(TransactionType.Delete), rank, entityEntry.Entity.ToString());

        change.NewData = string.Empty;
        change.OldData = JsonSerializer.Serialize(entityEntry.Entity);

        _changeWrapper.Changes.Add(change);
    }

    private void SetMainEntity(int rank, EntityEntry entityEntry)
    {
        if (rank is 1)
            _changeWrapper.MainEntity = entityEntry.Entity.ToString();
    }
}