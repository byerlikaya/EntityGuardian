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

        [EntityGuardian.DataAuditing]
        public async Task SavePublisher()
        {
            var publisher = new Publisher
            {
                Id = 1,
                Name = "Deneme",
                Books = new List<Book>
                {
                    new Book
                    {
                        Name = "Kitap 1",
                        Id = 1,
                        Price = 100
                    }
                }
            };

            await _context.Publishers.AddAsync(publisher);

            await _context.SaveChangesAsync();
        }

        [EntityGuardian.DataAuditing]
        public async Task UpdatePublisher()
        {
            var publisher = await _context.Publishers
                .Include(publisher => publisher.Books)
                .FirstOrDefaultAsync();

            publisher.Name = "Deneme 2";

            publisher.Books.FirstOrDefault()!.Name = "Kitap 2";

            await _context.SaveChangesAsync();
        }

        [EntityGuardian.DataAuditing]
        public async Task DeletePublisher()
        {
            var publisher = await _context.Publishers
                .FirstOrDefaultAsync();

            _context.Remove(publisher);

            await _context.SaveChangesAsync();
        }
    }
}
