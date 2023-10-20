namespace Sample.Api.ApplicationSpecific
{
    public interface ITestRepository
    {
        Task SavePublisher();

        Task UpdatePublisher();

        Task DeletePublisher();
    }
}
