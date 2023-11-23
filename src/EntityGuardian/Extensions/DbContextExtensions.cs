namespace EntityGuardian.Extensions;

public static class DbContextExtensions
{
    public static IEnumerable<EntityEntry> BringTheEntriesToBeAffected(this DbContext dbContext)
    {
        if (dbContext is null)
            return Enumerable.Empty<EntityEntry>();

        return dbContext.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
    }
}