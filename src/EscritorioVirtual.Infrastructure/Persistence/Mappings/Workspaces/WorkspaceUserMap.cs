using EscritorioVirtual.Domain.Aggregates.Workspaces;
using EscritorioVirtual.Infrastructure.Persistence.Mappings.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings.Workspaces;

public class WorkspaceUserMap : IEntityTypeConfiguration<WorkspaceUser>
{
    public void Configure(EntityTypeBuilder<WorkspaceUser> builder)
    {
        builder.ToTable("WorkspaceUsers", tb => tb.HasComment(ColumnCommentHelper.TableComment("vínculo entre usuários e workspaces")));

        builder.HasKey(x => new { x.WorkspaceId, x.UserId });

        builder.Property(x => x.WorkspaceId)
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("ID do Workspace"));

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasComment(ColumnCommentHelper.ColumnNotNull("ID do Usuário"));

        builder.Property(x => x.RoleId)
            .IsRequired()
            .HasConversion<int>()
            .HasComment(ColumnCommentHelper.ColumnNotNull("Papel do usuário (1=Owner, 2=Admin, 3=Member, 4=Guest)"));

        builder.Property(x => x.JoinedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment(ColumnCommentHelper.ColumnNotNull("Data de ingresso no Workspace"));

        builder.HasOne(x => x.Workspace)
            .WithMany(w => w.Members)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
