namespace EntityGuardian.Extensions;

internal static class DbContextExtensions
{
    public static IEnumerable<EntityEntry> BringTheEntriesToBeAffected(this DbContext dbContext) =>
        dbContext?.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList()
        ?? Enumerable.Empty<EntityEntry>();
}