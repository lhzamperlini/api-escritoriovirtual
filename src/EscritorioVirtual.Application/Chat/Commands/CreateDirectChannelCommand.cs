using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Commands;

public record CreateDirectChannelCommand(Guid WorkspaceId, Guid TargetUserId) : IRequest<ChatChannelDto>;

public class CreateDirectChannelCommandHandler(
    IChatRepository chatRepository,
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService) : IRequestHandler<CreateDirectChannelCommand, ChatChannelDto>
{
    public async Task<ChatChannelDto> Handle(CreateDirectChannelCommand request, CancellationToken cancellationToken)
    {
        var myUserId = currentUserService.UserId ?? Guid.Empty;
        var targetUser = await usuarioRepository.GetAsync(u => u.Id == request.TargetUserId, cancellationToken);
        var targetName = targetUser?.FullName ?? "Colega";

        var channel = await chatRepository.GetOrCreateDirectChannelAsync(
            request.WorkspaceId,
            myUserId,
            request.TargetUserId,
            targetName,
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
