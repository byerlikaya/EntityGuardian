using DataAuditing.Interfaces;
using EntityGuardian.Enums;

namespace DataAuditing.Options
{
    public class DataAuditingConfiguration : IDataAuditingConfiguration
    {
        public StorageType StorageType { get; set; }
        public string DashboardUrl { get; set; }
    }
}
