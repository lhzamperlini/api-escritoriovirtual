using EscritorioVirtual.Domain.AggregateRoot.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class MapMap : IEntityTypeConfiguration<Map>
{
    public void Configure(EntityTypeBuilder<Map> builder)
    {
        builder.ToTable("Maps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.WorkspaceId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.GridWidth)
            .HasDefaultValue(100)
            .IsRequired();

        builder.Property(x => x.GridHeight)
            .HasDefaultValue(100)
            .IsRequired();

        builder.Property(x => x.TileSize)
            .HasDefaultValue(32)
            .IsRequired();

        builder.Property(x => x.TiledMapData)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(x => x.Objects)
            .WithOne()
            .HasForeignKey(x => x.MapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Zones)
            .WithOne()
            .HasForeignKey(x => x.MapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
