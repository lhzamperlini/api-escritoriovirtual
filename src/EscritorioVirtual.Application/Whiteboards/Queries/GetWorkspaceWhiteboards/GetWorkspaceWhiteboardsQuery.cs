using EscritorioVirtual.Application.Whiteboards.DTOs;
using EscritorioVirtual.Application.Whiteboards.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Whiteboards.Queries.GetWorkspaceWhiteboards;

public record GetWorkspaceWhiteboardsQuery(Guid WorkspaceId) : IRequest<List<WhiteboardDto>>;

public class GetWorkspaceWhiteboardsQueryHandler(IWhiteboardRepository whiteboardRepository)
    : IRequestHandler<GetWorkspaceWhiteboardsQuery, List<WhiteboardDto>>
{
    public async Task<List<WhiteboardDto>> Handle(GetWorkspaceWhiteboardsQuery request, CancellationToken cancellationToken)
    {
        var boards = await whiteboardRepository.GetWorkspaceWhiteboardsAsync(request.WorkspaceId, cancellationToken);
        return boards.Select(b => new WhiteboardDto(
            b.Id,
            b.WorkspaceId,
            b.ZoneId,
            b.Name,
            b.DocumentData,
            b.CreatedAt,
            b.LastUpdated
        )).ToList();
    }
}
