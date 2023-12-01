namespace EntityGuardian.Options;

public class EntityGuardianOption
{

    private string _routePrefix;

    private string _entityGuardianSchemaName;

    private int _dataSynchronizationTimeout;

    public string RoutePrefix
    {
        get => string.IsNullOrWhiteSpace(_routePrefix)
            ? "entity-guardian"
            : _routePrefix;
        set => _routePrefix = value;
    }

    public bool ClearDataOnStartup { get; set; }

    public int DataSynchronizationTimeout
    {
        get => _dataSynchronizationTimeout is default(int)
            ? 30
            : _dataSynchronizationTimeout;
        set => _dataSynchronizationTimeout = value;
    }

    public StorageType StorageType { get; set; } = StorageType.SqlServer;

    public string EntityGuardianSchemaName
    {
        get => string.IsNullOrWhiteSpace(_entityGuardianSchemaName)
                ? "EntityGuardian"
                : _entityGuardianSchemaName;
        set => _entityGuardianSchemaName = value;
    }
}