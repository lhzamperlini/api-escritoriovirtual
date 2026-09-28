using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Commands;

public record MarkChannelAsReadCommand(Guid ChannelId) : IRequest<bool>;

public class MarkChannelAsReadCommandHandler(
    IChatRepository chatRepository,
    ICurrentUserService currentUserService) : IRequestHandler<MarkChannelAsReadCommand, bool>
{
    public async Task<bool> Handle(MarkChannelAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        if (userId != Guid.Empty)
        {
            await chatRepository.MarkChannelAsReadAsync(request.ChannelId, userId, cancellationToken);
            return true;
        }
        return false;
    }
}
