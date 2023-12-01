namespace Sample.Api.ApplicationSpecific;

public interface IPublisherRepository
{
    Task SavePublisher();

    Task UpdatePublisher();

    Task DeletePublisher();
}