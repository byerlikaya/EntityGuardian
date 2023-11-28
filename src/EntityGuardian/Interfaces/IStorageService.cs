namespace EntityGuardian.Interfaces;

public interface IStorageService
{
    Task CreateDatabaseTables();

    Task Synchronization(CancellationToken cancellationToken);

    Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest);

    Task<ResponseData<IEnumerable<Change>>> ChangesAsync(ChangesRequest searchRequest);

    Task<Change> ChangeAsync(Guid guid);
}