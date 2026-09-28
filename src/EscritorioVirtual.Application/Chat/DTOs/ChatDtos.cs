namespace EscritorioVirtual.Application.Chat.DTOs;

public record ChatChannelDto(
    Guid Id,
    Guid WorkspaceId,
    string ChannelType,
    Guid? ZoneId,
    string? Name,
    DateTime CreatedAt,
    int MemberCount
);

public record ChatMessageDto(
    Guid Id,
    Guid ChannelId,
    Guid SenderId,
    string SenderName,
    string Content,
    DateTime CreatedAt,
    bool IsRead
);
