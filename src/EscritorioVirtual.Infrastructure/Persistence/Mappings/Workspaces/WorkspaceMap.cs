using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Infrastructure.Persistence.Mappings.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings.Workspaces;

public class WorkspaceMap : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces", tb => tb.HasComment(ColumnCommentHelper.TableComment("workspaces/organizações da plataforma")));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("Identificador UUID do Workspace"));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment(ColumnCommentHelper.ColumnNotNull("Nome da empresa / organização"));

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment(ColumnCommentHelper.ColumnNotNull("Slug amigável para URL"));

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment(ColumnCommentHelper.ColumnNotNull("Data de criação"));

        builder.Property(x => x.DataCriacao)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Status)
            .HasDefaultValue(true);

        builder.HasMany(x => x.Members)
            .WithOne(m => m.Workspace)
            .HasForeignKey(m => m.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Invites)
            .WithOne(i => i.Workspace)
            .HasForeignKey(i => i.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

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
