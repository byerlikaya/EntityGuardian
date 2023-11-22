namespace EntityGuardian.Storages;

public class EntityGuardianDbContext : DbContext
{
    protected IConfiguration Configuration { get; }

    private readonly EntityGuardianOption _entityGuardianOption;

    public EntityGuardianDbContext(DbContextOptions<EntityGuardianDbContext> options, IConfiguration configuration, EntityGuardianOption option, EntityGuardianOption entityGuardianOption)
        : base(options)
    {
        Configuration = configuration;
        _entityGuardianOption = entityGuardianOption;
    }

    protected EntityGuardianDbContext(DbContextOptions options, IConfiguration configuration, EntityGuardianOption entityGuardianOption)
        : base(options)
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