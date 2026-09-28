using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Commands;

public record CreateZoneChannelCommand(Guid WorkspaceId, Guid ZoneId, string ZoneName) : IRequest<ChatChannelDto>;

public class CreateZoneChannelCommandHandler(IChatRepository chatRepository)
    : IRequestHandler<CreateZoneChannelCommand, ChatChannelDto>
{
    public async Task<ChatChannelDto> Handle(CreateZoneChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await chatRepository.GetOrCreateZoneChannelAsync(
            request.WorkspaceId,
            request.ZoneId,
            request.ZoneName,
            cancellationToken
        );

        return new ChatChannelDto(
            channel.Id,
            channel.WorkspaceId,
            channel.ChannelType,
            channel.ZoneId,
            channel.Name,
            channel.CreatedAt,
            channel.Members.Count
        );
    }
}
