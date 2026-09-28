using EscritorioVirtual.Domain.Aggregates.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings;

public class ChatChannelMemberMap : IEntityTypeConfiguration<ChatChannelMember>
{
    public void Configure(EntityTypeBuilder<ChatChannelMember> builder)
    {
        builder.ToTable("ChatChannelMembers");

        builder.HasKey(x => new { x.ChannelId, x.UserId });

        builder.Property(x => x.ChannelId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.LastReadAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.JoinedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => x.UserId);
    }
}
