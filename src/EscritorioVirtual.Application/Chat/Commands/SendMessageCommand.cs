using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.Aggregates.Chat;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Commands;

public record SendMessageCommand(Guid ChannelId, string Content) : IRequest<ChatMessageDto>;

public class SendMessageCommandHandler(
    IChatRepository chatRepository,
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService) : IRequestHandler<SendMessageCommand, ChatMessageDto>
{
    public async Task<ChatMessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var usuario = await usuarioRepository.GetAsync(u => u.Id == userId, cancellationToken);
        var senderName = usuario?.FullName ?? "Colega";

        var channel = await chatRepository.GetChannelByIdAsync(request.ChannelId, cancellationToken)
            ?? throw new InvalidOperationException("Canal de chat não encontrado.");

        var message = new ChatMessage(channel.Id, userId, senderName, request.Content);
        await chatRepository.AddMessageAsync(message, cancellationToken);

        return new ChatMessageDto(
            message.Id,
            message.ChannelId,
            message.SenderId,
            message.SenderName,
            message.Content,
            message.CreatedAt,
            message.IsRead
        );
    }
}
