using EscritorioVirtual.Domain.AggregateRoot.Chat;
using EscritorioVirtual.Domain.Aggregates.Chat;

namespace EscritorioVirtual.Application.Chat.Interfaces;

public interface IChatRepository
{
    Task<List<ChatChannel>> GetWorkspaceChannelsAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default);
    Task<ChatChannel?> GetChannelByIdAsync(Guid channelId, CancellationToken cancellationToken = default);
    Task<ChatChannel> GetOrCreateGlobalChannelAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task<ChatChannel> GetOrCreateZoneChannelAsync(Guid workspaceId, Guid zoneId, string zoneName, CancellationToken cancellationToken = default);
    Task<ChatChannel> GetOrCreateDirectChannelAsync(Guid workspaceId, Guid userA, Guid userB, string targetUserName, CancellationToken cancellationToken = default);
    Task<List<ChatMessage>> GetChannelMessagesAsync(Guid channelId, int take = 50, CancellationToken cancellationToken = default);
    Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task MarkChannelAsReadAsync(Guid channelId, Guid userId, CancellationToken cancellationToken = default);
}
