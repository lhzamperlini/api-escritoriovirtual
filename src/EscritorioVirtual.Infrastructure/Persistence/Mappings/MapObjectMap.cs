using EscritorioVirtual.Domain.Aggregates.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class MapObjectMap : IEntityTypeConfiguration<MapObject>
{
    public void Configure(EntityTypeBuilder<MapObject> builder)
    {
        builder.ToTable("MapObjects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.MapId)
            .IsRequired();

        builder.Property(x => x.AssetId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CoordX)
            .IsRequired();

        builder.Property(x => x.CoordY)
            .IsRequired();

        builder.Property(x => x.Rotation)
            .HasDefaultValue(0);

        builder.Property(x => x.IsSolid)
            .HasDefaultValue(true);

        builder.Property(x => x.ZIndexOffset)
            .HasDefaultValue(0);

        builder.HasIndex(x => x.MapId);
    }
}
