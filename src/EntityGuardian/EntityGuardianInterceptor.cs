namespace EntityGuardian;

public class EntityGuardianInterceptor : IInterceptor
{
    private ChangeWrapper _changeWrapper;
    private DbContext _dbContext;
    private readonly string _ipAddress;
    private readonly ICacheManager _cacheManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EntityGuardianInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ICacheManager cacheManager)
    {
        _httpContextAccessor = httpContextAccessor
                               ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cacheManager = cacheManager
                        ?? throw new ArgumentNullException(nameof(cacheManager));

        _ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        SetDbContext();
    }

    private void SetDbContext()
    {
        if (_httpContextAccessor.HttpContext?.Items["DbContext"] is not DbContext dbContext)
            return;

        dbContext.SavedChanges += DbContext_SavedChanges;
        dbContext.SavingChanges += DbContext_SavingChanges;
        dbContext.SaveChangesFailed += DbContext_SaveChangesFailed;

        _dbContext = dbContext;
    }

    public void Intercept(IInvocation invocation)
    {
        if (_dbContext is null)
            SetDbContext();

        var user = _httpContextAccessor.HttpContext?.User;

        _changeWrapper = new ChangeWrapper
        {
            Guid = Guid.NewGuid(),
            TargetName = invocation.TargetType.FullName,
            MethodName = invocation.Method.Name,
            IpAddress = _ipAddress,
            TransactionDate = DateTime.UtcNow,
            Username = user?.Identity?.Name ?? "undefined",
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

        var index = 1;

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
                        TransactionType = TransactionType.Insert.ToString(),
                        NewData = JsonSerializer.Serialize(entityEntry.Entity),
                        OldData = string.Empty,
                        TransactionDate = DateTime.UtcNow,
                        EntityName = entityEntry.Entity.ToString(),
                        Order = index
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
                            TransactionType = TransactionType.Update.ToString(),
                            NewData = JsonSerializer.Serialize(currentEntity),
                            OldData = JsonSerializer.Serialize(dbEntity),
                            EntityName = entityEntry.Entity.ToString(),
                            TransactionDate = DateTime.UtcNow,
                            Order = index
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
                        TransactionType = TransactionType.Delete.ToString(),
                        NewData = string.Empty,
                        OldData = JsonSerializer.Serialize(entityEntry.Entity),
                        TransactionDate = DateTime.UtcNow,
                        EntityName = entityEntry.Entity.ToString(),
                        Order = index
                    });
                    break;
                }
            }
            index++;
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