using EntityGuardian.Enums;
using EntityGuardian.Interfaces;

namespace EntityGuardian.Options
{
    public class EntityGuardianConfiguration : IEntityGuardianConfiguration
    {
        public StorageType StorageType { get; set; }
        public string DashboardUrl { get; set; }
    }
}
