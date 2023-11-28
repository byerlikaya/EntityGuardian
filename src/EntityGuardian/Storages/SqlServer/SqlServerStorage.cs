namespace EntityGuardian.Storages.SqlServer;

internal class SqlServerStorage(
    ICacheManager cacheManager,
    EntityGuardianOption options,
    EntityGuardianDbContext context) : IStorageService
{
    public async Task CreateDatabaseTables()
    {
        await context.Database.ExecuteSqlRawAsync(GetSqlScript(options.EntityGuardianSchemaName));

        if (!options.ClearDataOnStartup)
            return;

        await context.Database.ExecuteSqlRawAsync($"DELETE FROM {SchemaName(options.EntityGuardianSchemaName)}.Change");
        await context.Database.ExecuteSqlRawAsync($"DELETE FROM {SchemaName(options.EntityGuardianSchemaName)}.ChangeWrapper");
    }

    public async Task Synchronization(CancellationToken cancellationToken)
    {
        if (MemoryDataControl(out var memoryData))
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await context.ChangeWrapper.AddRangeAsync(memoryData.Select(x => x.data).ToList(), cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                transaction.Complete();

                memoryData.ForEach(x => cacheManager.Remove(x.key));
            }
            catch
            {
                transaction.Dispose();
            }
        }
    }

    public async Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest)
    {
        var query = context.ChangeWrapper
            .Include(x => x.Changes)
            .Where(searchRequest);

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
        var query = context.Change.Where(searchRequest);

        var count = await query.CountAsync();

        var result = await query
            .OrderBy(searchRequest.OrderBy)
            .Skip(searchRequest.Start)
            .Take(searchRequest.Max == default ? 10 : searchRequest.Max)
            .ToListAsync();

        return new ResponseData<IEnumerable<Change>>(result, count);
    }

    public async Task<Change> ChangeAsync(Guid guid)
    {
        return await context.Change.FirstOrDefaultAsync(x => x.Guid == guid);
    }

    private bool MemoryDataControl(out List<(string key, ChangeWrapper data)> memoryData)
    {
        memoryData = cacheManager.GetList<ChangeWrapper>(nameof(ChangeWrapper));
        return memoryData.Any();
    }

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