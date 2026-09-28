using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class WhiteboardMap : IEntityTypeConfiguration<Whiteboard>
{
    public void Configure(EntityTypeBuilder<Whiteboard> builder)
    {
        builder.ToTable("Whiteboards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.WorkspaceId)
            .IsRequired();

        builder.Property(x => x.ZoneId)
            .IsRequired(false);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DocumentData)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.LastUpdated)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => x.WorkspaceId);
        builder.HasIndex(x => x.ZoneId);
        builder.HasIndex(x => new { x.WorkspaceId, x.ZoneId });
    }
}
