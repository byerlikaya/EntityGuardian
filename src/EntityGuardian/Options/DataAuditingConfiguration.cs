using EntityGuardian.Enums;
using EntityGuardian.Interfaces;

namespace EntityGuardian.Options
{
    public class DataAuditingConfiguration : IDataAuditingConfiguration
    {
        public StorageType StorageType { get; set; }
        public string DashboardUrl { get; set; }
    }
}
