using EscritorioVirtual.Application.Whiteboards.DTOs;
using EscritorioVirtual.Application.Whiteboards.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Whiteboards.Commands.SaveSnapshot;

public record SaveWhiteboardSnapshotCommand(Guid Id, string DocumentData) : IRequest<WhiteboardDto?>;

public class SaveWhiteboardSnapshotCommandHandler(IWhiteboardRepository whiteboardRepository)
    : IRequestHandler<SaveWhiteboardSnapshotCommand, WhiteboardDto?>
{
    public async Task<WhiteboardDto?> Handle(SaveWhiteboardSnapshotCommand request, CancellationToken cancellationToken)
    {
        var board = await whiteboardRepository.GetByIdAsync(request.Id, cancellationToken);
        if (board is null) return null;

        board.UpdateSnapshot(request.DocumentData);
        await whiteboardRepository.UpdateAsync(board, cancellationToken);

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
