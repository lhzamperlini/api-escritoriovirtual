namespace EscritorioVirtual.Domain.Aggregates.Chat;

public class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public Guid SenderId { get; private set; }
    public string SenderName { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    protected ChatMessage() { }

    public ChatMessage(Guid channelId, Guid senderId, string senderName, string content, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        ChannelId = channelId;
        SenderId = senderId;
        SenderName = senderName.Trim();
        Content = content.Trim();
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
