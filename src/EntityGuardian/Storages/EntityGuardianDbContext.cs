namespace EntityGuardian.Storages;

internal class EntityGuardianDbContext : DbContext
{
    private IConfiguration Configuration { get; }

    private readonly EntityGuardianOption _entityGuardianOption;

    protected EntityGuardianDbContext(
        DbContextOptions<EntityGuardianDbContext> options,
        IConfiguration configuration,
        EntityGuardianOption entityGuardianOption) : base(options)
    {
        Configuration = configuration;
        _entityGuardianOption = entityGuardianOption;
    }

    public EntityGuardianDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        EntityGuardianOption entityGuardianOption) : base(options)
    {
        Configuration = configuration;
        _entityGuardianOption = entityGuardianOption;
    }

    public DbSet<ChangeWrapper> ChangeWrapper { get; set; }

    public DbSet<Change> Change { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_entityGuardianOption.EntityGuardianSchemaName);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            base.OnConfiguring(optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
        }
    }
}