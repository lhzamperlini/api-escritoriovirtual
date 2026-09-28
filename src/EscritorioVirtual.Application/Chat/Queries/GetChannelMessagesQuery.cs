using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Chat.Queries;

public record GetChannelMessagesQuery(Guid ChannelId, int Take = 50) : IRequest<List<ChatMessageDto>>;

public class GetChannelMessagesQueryHandler(IChatRepository chatRepository)
    : IRequestHandler<GetChannelMessagesQuery, List<ChatMessageDto>>
{
    public async Task<List<ChatMessageDto>> Handle(GetChannelMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await chatRepository.GetChannelMessagesAsync(request.ChannelId, request.Take, cancellationToken);

        return messages.Select(m => new ChatMessageDto(
            m.Id,
            m.ChannelId,
            m.SenderId,
            m.SenderName,
            m.Content,
            m.CreatedAt,
            m.IsRead
        )).ToList();
    }
}
