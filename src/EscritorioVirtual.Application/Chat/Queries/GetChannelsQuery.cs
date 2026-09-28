using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Queries;

public record GetChannelsQuery(Guid WorkspaceId) : IRequest<List<ChatChannelDto>>;

public class GetChannelsQueryHandler(
    IChatRepository chatRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetChannelsQuery, List<ChatChannelDto>>
{
    public async Task<List<ChatChannelDto>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;

        // Ensure default global channel exists
        await chatRepository.GetOrCreateGlobalChannelAsync(request.WorkspaceId, cancellationToken);

        var channels = await chatRepository.GetWorkspaceChannelsAsync(request.WorkspaceId, userId, cancellationToken);

        return channels.Select(c => new ChatChannelDto(
            c.Id,
            c.WorkspaceId,
            c.ChannelType,
            c.ZoneId,
            c.Name,
            c.CreatedAt,
            c.Members.Count
        )).ToList();
    }
}
