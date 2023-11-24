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
        _context.Database.ExecuteSqlRaw(GetSqlScript(_options.EntityGuardianSchemaName));

        if (!_options.ClearDataOnStartup)
            return;

        _context.Database.ExecuteSqlRaw($"DELETE FROM {SchemaName(_options.EntityGuardianSchemaName)}.Change");
        _context.Database.ExecuteSqlRaw($"DELETE FROM {SchemaName(_options.EntityGuardianSchemaName)}.ChangeWrapper");
    }

    public async Task Synchronization(CancellationToken cancellationToken)
    {
        if (MemoryDataControl(out var memoryData))
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await _context.ChangeWrapper.AddRangeAsync(memoryData.Select(x => x.data).ToList(), cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                transaction.Complete();

                memoryData.ForEach(x => _cacheManager.Remove(x.key));
            }
            catch
            {
                transaction.Dispose();
            }
        }
    }

    private bool MemoryDataControl(out List<(string key, ChangeWrapper data)> memoryData)
    {
        memoryData = _cacheManager.GetList<ChangeWrapper>(nameof(ChangeWrapper));
        return memoryData.Any();
    }

    public async Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest)
    {
        var query = _context.ChangeWrapper.Where(searchRequest);

        var count = await query.CountAsync();

        var result = await query
            .OrderBy(searchRequest.OrderBy)
            .Skip(searchRequest.Start)
            .Take(searchRequest.Max is default(int) ? 10 : searchRequest.Max)
            .ToListAsync();

        return new ResponseData<IEnumerable<ChangeWrapper>>(result, count);
    }

    public async Task<ResponseData<IEnumerable<Change>>> ChangesAsync(ChangesRequest searchRequest)
    {
        var query = _context.Change.Where(searchRequest);

        var count = await query.CountAsync();

        var result = await query
            .OrderBy(searchRequest.OrderBy)
            .Skip(searchRequest.Start)
            .Take(searchRequest.Max == default ? 10 : searchRequest.Max)
            .ToListAsync();

        return new ResponseData<IEnumerable<Change>>(result, count);
    }

    public async Task<Change> ChangeAsync(Guid guid) =>
        await _context.Change.FirstOrDefaultAsync(x => x.Guid == guid);

    private static string GetSqlScript(string schema) =>
        GetStringResource(typeof(SqlServerStorage).GetTypeInfo().Assembly, "EntityGuardian.Storages.SqlServer.Install.sql")
            .Replace("$(EntityGuardiaonSchemaName)", schema);

    private static string GetStringResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string SchemaName(string schema) =>
        string.IsNullOrWhiteSpace(schema)
            ? "EntityGuardian"
            : schema;
}