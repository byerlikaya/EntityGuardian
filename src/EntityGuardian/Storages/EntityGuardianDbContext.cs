using EntityGuardian.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityGuardian.Storages
{
    public class EntityGuardianDbContext : DbContext
    {
        protected IConfiguration Configuration { get; }

        public EntityGuardianDbContext(DbContextOptions<EntityGuardianDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected EntityGuardianDbContext(DbContextOptions options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public DbSet<ChangeWrapper> ChangeWrapper { get; set; }

        public DbSet<Change> Change { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
            }
        }
    }
}
