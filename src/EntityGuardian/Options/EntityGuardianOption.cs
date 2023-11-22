namespace EntityGuardian.Options;

public class EntityGuardianOption
{

    private string _routePrefix;

    public string RoutePrefix
    {
        get => string.IsNullOrEmpty(_routePrefix)
            ? "entity-guardian"
            : _routePrefix;
        set => _routePrefix = value;
    }

    public bool ClearDataOnStartup { get; set; }

    /// <summary>
    /// will be defined in seconds.
    /// </summary>
    public int DataSynchronizationTimeout { get; set; }

    public StorageType StorageType { get; set; }

    private string _entityGuardianSchemaName;

    public string EntityGuardianSchemaName
    {
        get => string.IsNullOrEmpty(_entityGuardianSchemaName)
                ? "EntityGuardian"
                : _entityGuardianSchemaName;
        set => _entityGuardianSchemaName = value;
    }
}