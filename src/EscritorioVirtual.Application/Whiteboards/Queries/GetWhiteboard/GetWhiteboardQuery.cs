using EscritorioVirtual.Application.Whiteboards.DTOs;
using EscritorioVirtual.Application.Whiteboards.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;
using MediatR;

namespace EscritorioVirtual.Application.Whiteboards.Queries.GetWhiteboard;

public record GetWhiteboardQuery(Guid? Id, Guid? WorkspaceId, Guid? ZoneId) : IRequest<WhiteboardDto?>;

public class GetWhiteboardQueryHandler(IWhiteboardRepository whiteboardRepository) 
    : IRequestHandler<GetWhiteboardQuery, WhiteboardDto?>
{
    public async Task<WhiteboardDto?> Handle(GetWhiteboardQuery request, CancellationToken cancellationToken)
    {
        Whiteboard? board = null;

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            board = await whiteboardRepository.GetByIdAsync(request.Id.Value, cancellationToken);
        }
        else if (request.WorkspaceId.HasValue && request.ZoneId.HasValue)
        {
            board = await whiteboardRepository.GetByZoneIdAsync(request.WorkspaceId.Value, request.ZoneId.Value, cancellationToken);

            // Se ainda não existir para esta zona, cria um padrão vazio
            if (board is null)
            {
                var newBoard = new Whiteboard(request.WorkspaceId.Value, "Quadro da Sala", request.ZoneId.Value);
                board = await whiteboardRepository.CreateAsync(newBoard, cancellationToken);
            }
        }

        if (board is null) return null;

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
