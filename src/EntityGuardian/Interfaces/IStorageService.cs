namespace EntityGuardian.Interfaces;

public interface IStorageService
{
    Task Synchronization();

    Task<ResponseData<IEnumerable<ChangeWrapper>>> ChangeWrappersAsync(ChangeWrapperRequest searchRequest);

    Task<ResponseData<IEnumerable<Change>>> ChangesAsync(ChangesRequest searchRequest);

    Task<Change> ChangeAsync(Guid guid);
}