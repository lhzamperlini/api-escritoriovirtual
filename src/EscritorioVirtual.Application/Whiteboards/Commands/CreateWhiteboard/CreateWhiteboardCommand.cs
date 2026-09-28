using EscritorioVirtual.Application.Whiteboards.DTOs;
using EscritorioVirtual.Application.Whiteboards.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;
using MediatR;

namespace EscritorioVirtual.Application.Whiteboards.Commands.CreateWhiteboard;

public record CreateWhiteboardCommand(
    Guid WorkspaceId,
    string Name,
    Guid? ZoneId = null,
    string? InitialData = null
) : IRequest<WhiteboardDto>;

public class CreateWhiteboardCommandHandler(IWhiteboardRepository whiteboardRepository)
    : IRequestHandler<CreateWhiteboardCommand, WhiteboardDto>
{
    public async Task<WhiteboardDto> Handle(CreateWhiteboardCommand request, CancellationToken cancellationToken)
    {
        var board = new Whiteboard(
            request.WorkspaceId,
            request.Name,
            request.ZoneId,
            request.InitialData
        );

        await whiteboardRepository.CreateAsync(board, cancellationToken);

        return new WhiteboardDto(
            board.Id,
            board.WorkspaceId,
            board.ZoneId,
            board.Name,
            board.DocumentData,
            board.CreatedAt,
            board.LastUpdated
        );
    }
}
