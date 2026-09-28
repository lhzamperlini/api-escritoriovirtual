using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EscritorioVirtual.Infrastructure.Persistence.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<EscritorioVirtual.Domain.AggregateRoot.Chat.ChatChannel> ChatChannels => Set<EscritorioVirtual.Domain.AggregateRoot.Chat.ChatChannel>();
	public DbSet<EscritorioVirtual.Domain.Aggregates.Chat.ChatChannelMember> ChatChannelMembers => Set<EscritorioVirtual.Domain.Aggregates.Chat.ChatChannelMember>();
	public DbSet<EscritorioVirtual.Domain.Aggregates.Chat.ChatMessage> Messages => Set<EscritorioVirtual.Domain.Aggregates.Chat.ChatMessage>();

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