namespace EntityGuardian;

[AttributeUsage(AttributeTargets.Method)]
public class EntityGuardian : Attribute, IInterceptor
{
    private ChangeWrapper _changeWrapper;
    private readonly DbContext _dbContext;
    private readonly string _ipAddress;
    private readonly ICacheManager _cacheManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EntityGuardian()
    {
        _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

        _ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        if (_httpContextAccessor.HttpContext?.Items["DbContext"] is not DbContext dbContext)
            return;

        _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();

        dbContext.SavedChanges += DbContext_SavedChanges;
        dbContext.SavingChanges += DbContext_SavingChanges;
        dbContext.SaveChangesFailed += DbContext_SaveChangesFailed;

        _dbContext = dbContext;
    }

    public void Intercept(IInvocation invocation)
    {
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
                        TransactionType = TransactionType.INSERT.ToString(),
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
                            TransactionType = TransactionType.UPDATE.ToString(),
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
                        TransactionType = TransactionType.DELETE.ToString(),
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