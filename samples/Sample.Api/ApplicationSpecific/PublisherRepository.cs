namespace Sample.Api.ApplicationSpecific;

public class PublisherRepository(MemoryDbContext context) : IPublisherRepository
{
    public async Task SavePublisher()
    {
        var publisher = new Publisher
        {
            Id = new Random().Next(1, 99999),
            Name = "Publisher",
            Books =
            [
                new() {
                    Name = "Book 1",
                    Id = new Random().Next(1, 99999),
                    Price = 100,
                    Author = new Author
                    {
                        Name = "Author 1",
                        Id=new Random().Next(1, 99999),
                        Age = 30
                    }
                }
            ]
        };

        await context.Publishers.AddAsync(publisher);

        await context.SaveChangesAsync();
    }

    public async Task UpdatePublisher()
    {
        var publisher = await context.Publishers
            .Include(publisher => publisher.Books)
            .FirstOrDefaultAsync();

        publisher.Name = "Publisher 2";

        publisher.Books.FirstOrDefault()!.Name = "Book 2";

        await context.SaveChangesAsync();
    }


    public async Task DeletePublisher()
    {
        var publisher = await context.Publishers.FirstOrDefaultAsync();

        context.Remove(publisher);

        await context.SaveChangesAsync();
    }
}