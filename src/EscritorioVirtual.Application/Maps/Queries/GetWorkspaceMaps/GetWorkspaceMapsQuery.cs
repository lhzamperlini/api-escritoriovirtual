using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Maps.Queries.GetWorkspaceMaps;

public record GetWorkspaceMapsQuery(Guid WorkspaceId) : IRequest<List<MapDto>>;

public class GetWorkspaceMapsQueryHandler(IMapRepository mapRepository)
    : IRequestHandler<GetWorkspaceMapsQuery, List<MapDto>>
{
    public async Task<List<MapDto>> Handle(GetWorkspaceMapsQuery request, CancellationToken cancellationToken)
    {
        var maps = await mapRepository.GetMapsByWorkspaceAsync(request.WorkspaceId, cancellationToken);
        return maps.Select(m => new MapDto
        {
            Id = m.Id,
            WorkspaceId = m.WorkspaceId,
            Name = m.Name,
            GridWidth = m.GridWidth,
            GridHeight = m.GridHeight,
            TileSize = m.TileSize,
            TiledMapData = m.TiledMapData,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }
}
