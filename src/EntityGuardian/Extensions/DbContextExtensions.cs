using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityGuardian.Extensions;

public static class DbContextExtensions
{
    public static List<EntityEntry> BringTheEntriesToBeAffected(this DbContext dbContext) =>
        dbContext.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
}