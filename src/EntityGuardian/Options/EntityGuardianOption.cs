namespace EntityGuardian.Options;

public class EntityGuardianOption
{

    private string _routePrefix;

    private string _entityGuardianSchemaName;

    public string RoutePrefix
    {
        get => string.IsNullOrEmpty(_routePrefix)
            ? "entity-guardian"
            : _routePrefix;
        set => _routePrefix = value;
    }

    public bool ClearDataOnStartup { get; set; }

    public int DataSynchronizationTimeout { get; set; } = 30;

    public StorageType StorageType { get; set; } = StorageType.SqlServer;

    public string EntityGuardianSchemaName
    {
        get => string.IsNullOrEmpty(_entityGuardianSchemaName)
                ? "EntityGuardian"
                : _entityGuardianSchemaName;
        set => _entityGuardianSchemaName = value;
    }
}