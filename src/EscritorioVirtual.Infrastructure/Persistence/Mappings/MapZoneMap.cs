using EscritorioVirtual.Domain.Aggregates.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class MapZoneMap : IEntityTypeConfiguration<MapZone>
{
    public void Configure(EntityTypeBuilder<MapZone> builder)
    {
        builder.ToTable("MapZones");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.MapId)
            .IsRequired();

        builder.Property(x => x.ZoneType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.StartX)
            .IsRequired();

        builder.Property(x => x.StartY)
            .IsRequired();

        builder.Property(x => x.EndX)
            .IsRequired();

        builder.Property(x => x.EndY)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired(false);

        builder.HasIndex(x => x.MapId);
    }
}
