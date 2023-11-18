namespace EntityGuardian.Storages.SqlServer;

internal class SqlServerStorage : IStorageService
{
    private readonly ICacheManager _cacheManager;
    private readonly EntityGuardianDbContext _context;
    private readonly EntityGuardianOption _options;

    public SqlServerStorage(ICacheManager cacheManager, EntityGuardianOption options)
    {
        _context = ServiceTool.ServiceProvider.GetService<EntityGuardianDbContext>();
        _cacheManager = cacheManager;
        _options = options;
        CreateDatabaseTables();
    }

    public void CreateDatabaseTables()
    {
        _context.Database.ExecuteSqlRaw(GetSqlScript(_options.EntityGuardiaonSchemaName));

        if (!_options.ClearDataOnStartup)
            return;

        _context.Database.ExecuteSqlRaw($"DELETE FROM {SchemaName(_options.EntityGuardiaonSchemaName)}.Change");
        _context.Database.ExecuteSqlRaw($"DELETE FROM {SchemaName(_options.EntityGuardiaonSchemaName)}.ChangeWrapper");
    }

    public async Task Synchronization()
    {
        var memoryData = _cacheManager.GetList<ChangeWrapper>(nameof(ChangeWrapper));

        if (memoryData.Any())
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var changeWrappers = memoryData
                    .Select(x => x.data)
                    .ToList();

                await _context.ChangeWrapper.AddRangeAsync(changeWrappers);

                await _context.SaveChangesAsync();

                transaction.Complete();

                memoryData.ForEach(x => _cacheManager.Remove(x.key));
            }
            catch
            {
                transaction.Dispose();
            }
        }
    }

    public async Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest)
    {
        var query = _context.ChangeWrapper
            .Where(searchRequest)
            .OrderBy(searchRequest.OrderBy);

        var count = await query.CountAsync();

        var result = await query
            .Skip(searchRequest.Start)
            .Take(searchRequest.Max == default ? 10 : searchRequest.Max)
            .ToListAsync();

        return new ResponseData<IEnumerable<ChangeWrapper>>(result, count);
    }

    public async Task<ResponseData<IEnumerable<Change>>> ChangesAsync(ChangesRequest searchRequest)
    {
        var query = _context.Change
            .Where(searchRequest)
            .OrderBy(searchRequest.OrderBy);

        var count = await query.CountAsync();

        var result = await query
            .Skip(searchRequest.Start)
            .Take(searchRequest.Max == default ? 10 : searchRequest.Max)
            .ToListAsync();

        return new ResponseData<IEnumerable<Change>>(result, count);
    }

    public async Task<Change> ChangeAsync(Guid guid)
        => await _context.Change.FirstOrDefaultAsync(x => x.Guid == guid);

    private static string GetSqlScript(string schema)
    {
        var script = GetStringResource(typeof(SqlServerStorage).GetTypeInfo().Assembly,
            "EntityGuardian.Storages.SqlServer.Install.sql");

        script = script.Replace("$(EntityGuardiaonSchemaName)", schema);

        return script;
    }

    private static string GetStringResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string SchemaName(string schema)
    {
        return !string.IsNullOrWhiteSpace(schema)
            ? schema
            : "EntityGuardian";
    }
}