namespace EntityGuardian.Storages;

internal class EntityGuardianDbContext : DbContext
{

    private readonly EntityGuardianOption _entityGuardianOption;

    protected EntityGuardianDbContext(DbContextOptions<EntityGuardianDbContext> options, EntityGuardianOption entityGuardianOption) : base(options) =>
        _entityGuardianOption = entityGuardianOption;

    public EntityGuardianDbContext(DbContextOptions options, EntityGuardianOption entityGuardianOption) : base(options) =>
        _entityGuardianOption = entityGuardianOption;

    public DbSet<ChangeWrapper> ChangeWrapper { get; set; }

    public DbSet<Change> Change { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_entityGuardianOption.EntityGuardianSchemaName);
        base.OnModelCreating(modelBuilder);
    }
}