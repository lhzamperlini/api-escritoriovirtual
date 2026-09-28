using EscritorioVirtual.Domain.Aggregates.Chat;

namespace EscritorioVirtual.Domain.AggregateRoot.Chat;

public class ChatChannel : BaseAggregateRoot<Guid>
{
    public Guid WorkspaceId { get; private set; }
    public string ChannelType { get; private set; } = "Global"; // Global, Zone, Direct
    public Guid? ZoneId { get; private set; }
    public string? Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ChatChannelMember> _members = new();
    public virtual IReadOnlyCollection<ChatChannelMember> Members => _members.AsReadOnly();

    private readonly List<ChatMessage> _messages = new();
    public virtual IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    protected ChatChannel() { }

    public ChatChannel(Guid workspaceId, string channelType, string? name = null, Guid? zoneId = null, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        WorkspaceId = workspaceId;
        ChannelType = channelType;
        Name = name?.Trim();
        ZoneId = zoneId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DataCriacao = DateTime.UtcNow;
        Status = true;
    }

    public void AddMember(Guid userId)
    {
        if (!_members.Any(m => m.UserId == userId))
        {
            _members.Add(new ChatChannelMember(Id, userId));
            UpdatedAt = DateTime.UtcNow;
            DataAtualizacao = DateTime.UtcNow;
        }
    }

    public ChatMessage AddMessage(Guid senderId, string senderName, string content)
    {
        var msg = new ChatMessage(Id, senderId, senderName, content);
        _messages.Add(msg);
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
        return msg;
    }

    public void MarkMemberRead(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        member?.MarkRead();
    }
}
