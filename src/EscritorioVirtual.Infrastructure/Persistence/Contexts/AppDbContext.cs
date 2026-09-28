using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EscritorioVirtual.Infrastructure.Persistence.Contexts;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EscritorioVirtual.Domain.AggregateRoot.Usuario> Usuarios { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		_ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

		base.OnModelCreating(modelBuilder);

		_ = modelBuilder.HasDefaultSchema("public");
		_ = modelBuilder.HasPostgresExtension("uuid-ossp");
		_ = modelBuilder.UseSerialColumns();
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);

		configurationBuilder.Conventions.Remove<CascadeDeleteConvention>();
		_ = configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
	}
}