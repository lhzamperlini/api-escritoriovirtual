using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Maps.Commands.ImportTiledMap;

public record ImportTiledMapCommand(Guid MapId, string TiledMapJson) : IRequest<MapDto>;

public class ImportTiledMapCommandValidator : AbstractValidator<ImportTiledMapCommand>
{
    public ImportTiledMapCommandValidator()
    {
        RuleFor(x => x.MapId).NotEmpty().WithMessage("O ID do mapa é obrigatório.");
        RuleFor(x => x.TiledMapJson).NotEmpty().WithMessage("O JSON do Tiled é obrigatório.");
    }
}

public class ImportTiledMapCommandHandler(IMapRepository mapRepository)
    : IRequestHandler<ImportTiledMapCommand, MapDto>
{
    public async Task<MapDto> Handle(ImportTiledMapCommand request, CancellationToken cancellationToken)
    {
        var map = await mapRepository.GetMapWithDetailsAsync(request.MapId, cancellationToken);
        if (map is null)
        {
            throw new NotFoundException("Mapa não encontrado.");
        }

        map.ImportTiledData(request.TiledMapJson);
        await mapRepository.UpdateMapAsync(map, cancellationToken);

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
