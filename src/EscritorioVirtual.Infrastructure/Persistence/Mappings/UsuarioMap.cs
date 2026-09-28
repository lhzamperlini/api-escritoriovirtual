using EscritorioVirtual.Domain.AggregateRoot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.LastLoginAt);

        builder.Property(x => x.AvatarConfig)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasOne(x => x.UsuarioCriacao)
            .WithMany()
            .HasForeignKey(x => x.UsuarioCriacaoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UsuarioAtualizacao)
            .WithMany()
            .HasForeignKey(x => x.UsuarioAtualizacaoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
