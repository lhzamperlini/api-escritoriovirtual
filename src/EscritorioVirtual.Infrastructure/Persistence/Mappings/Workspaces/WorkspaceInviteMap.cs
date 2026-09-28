using EscritorioVirtual.Domain.Aggregates.Workspaces;
using EscritorioVirtual.Infrastructure.Persistence.Mappings.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings.Workspaces;

public class WorkspaceInviteMap : IEntityTypeConfiguration<WorkspaceInvite>
{
    public void Configure(EntityTypeBuilder<WorkspaceInvite> builder)
    {
        builder.ToTable("WorkspaceInvites", tb => tb.HasComment(ColumnCommentHelper.TableComment("convites para acesso ao Workspace")));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("Identificador UUID do convite"));

        builder.Property(x => x.WorkspaceId)
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("ID do Workspace"));

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment(ColumnCommentHelper.ColumnNotNull("Código único do convite"));

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .HasComment(ColumnCommentHelper.ColumnNull("E-mail destinatário (opcional)"));

        builder.Property(x => x.RoleId)
            .IsRequired()
            .HasConversion<int>()
            .HasComment(ColumnCommentHelper.ColumnNotNull("Papel atribuído"));

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment(ColumnCommentHelper.ColumnNotNull("Data de geração do convite"));

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("ID do usuário criador"));

        builder.Property(x => x.ExpiresAt)
            .HasComment(ColumnCommentHelper.ColumnNull("Data de expiração"));

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .HasComment(ColumnCommentHelper.ColumnNotNull("Status ativo"));

        builder.HasOne(x => x.Workspace)
            .WithMany(w => w.Invites)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
