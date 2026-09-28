using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Maps.Queries.GetMapById;

public record GetMapByIdQuery(Guid MapId) : IRequest<MapDto>;

public class GetMapByIdQueryHandler(IMapRepository mapRepository)
    : IRequestHandler<GetMapByIdQuery, MapDto>
{
    public async Task<MapDto> Handle(GetMapByIdQuery request, CancellationToken cancellationToken)
    {
        var map = await mapRepository.GetMapWithDetailsAsync(request.MapId, cancellationToken);
        if (map is null)
        {
            throw new NotFoundException("Mapa não encontrado.");
        }

        return new MapDto
        {
            Id = map.Id,
            WorkspaceId = map.WorkspaceId,
            Name = map.Name,
            GridWidth = map.GridWidth,
            GridHeight = map.GridHeight,
            TileSize = map.TileSize,
            TiledMapData = map.TiledMapData,
            CreatedAt = map.CreatedAt,
            UpdatedAt = map.UpdatedAt,
            Objects = map.Objects.Select(o => new MapObjectDto
            {
                Id = o.Id,
                MapId = o.MapId,
                AssetId = o.AssetId,
                CoordX = o.CoordX,
                CoordY = o.CoordY,
                Rotation = o.Rotation,
                IsSolid = o.IsSolid,
                ZIndexOffset = o.ZIndexOffset
            }).ToList(),
            Zones = map.Zones.Select(z => new MapZoneDto
            {
                Id = z.Id,
                MapId = z.MapId,
                ZoneType = z.ZoneType,
                Name = z.Name,
                StartX = z.StartX,
                StartY = z.StartY,
                EndX = z.EndX,
                EndY = z.EndY,
                Capacity = z.Capacity
            }).ToList()
        };
    }
}
