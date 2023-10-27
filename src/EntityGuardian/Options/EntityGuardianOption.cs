using EntityGuardian.Enums;

namespace EntityGuardian.Options
{
    public class EntityGuardianOption
    {

        public string RoutePrefix { get; set; } = "entity-guardian";

        public bool ClearDataOnStartup { get; set; }

        /// <summary>
        /// will be defined in seconds.
        /// </summary>
        public int DataSynchronizationTimeout { get; set; }

        public StorageType StorageType { get; set; }
    }
}
