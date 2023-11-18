using Microsoft.EntityFrameworkCore;
using Sample.Api.ApplicationSpecific.Contexts;
using Sample.DomainObjects.Entity;

namespace Sample.Api.ApplicationSpecific
{
    public class TestRepository : ITestRepository
    {
        private readonly MemoryDbContext _context;

        public TestRepository(MemoryDbContext context)
        {
            _context = context;
        }

        public async Task SavePublisher()
        {
            var publisher = new Publisher
            {
                Id = new Random().Next(1, 99999),
                Name = "Deneme",
                Books = new List<Book>
                {
                    new Book
                    {
                        Name = "Kitap 1",
                        Id = new Random().Next(1, 99999),
                        Price = 100,
                        Author = new Author
                        {
                            Name = "Yazar 1",
                            Id=new Random().Next(1, 99999),
                            Age = 30
                        }
                    }
                }
            };

            await _context.Publishers.AddAsync(publisher);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePublisher()
        {
            var publisher = await _context.Publishers
                .Include(publisher => publisher.Books)
                .FirstOrDefaultAsync();

            publisher.Name = "Deneme 2";

            publisher.Books.FirstOrDefault()!.Name = "Kitap 2";

            await _context.SaveChangesAsync();
        }


        public async Task DeletePublisher()
        {
            var publisher = await _context.Publishers
                .FirstOrDefaultAsync();

            _context.Remove(publisher);

            await _context.SaveChangesAsync();
        }
    }
}
