using EntityGuardian.Interfaces;
using EntityGuardian.Storages;
using System;

namespace EntityGuardian.Extensions
{
    public static class StorageExtensions
    {
        public static IEntityGuardianConfiguration UseSqlServerStorage(this IEntityGuardianConfiguration options,
            string nameOrConnectionString)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (nameOrConnectionString == null) throw new ArgumentNullException(nameof(nameOrConnectionString));

            var storage = new Storage(nameOrConnectionString);
            storage.Initialize();

            return options;
        }
    }
}
