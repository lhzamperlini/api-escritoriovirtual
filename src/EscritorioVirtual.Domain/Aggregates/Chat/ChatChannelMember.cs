namespace EscritorioVirtual.Domain.Aggregates.Chat;

public class ChatChannelMember
{
    public Guid ChannelId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime LastReadAt { get; private set; }
    public DateTime JoinedAt { get; private set; }

    protected ChatChannelMember() { }

    public ChatChannelMember(Guid channelId, Guid userId, DateTime? joinedAt = null)
    {
        ChannelId = channelId;
        UserId = userId;
        JoinedAt = joinedAt ?? DateTime.UtcNow;
        LastReadAt = joinedAt ?? DateTime.UtcNow;
    }

    public void MarkRead(DateTime? readAt = null)
    {
        LastReadAt = readAt ?? DateTime.UtcNow;
    }
}
