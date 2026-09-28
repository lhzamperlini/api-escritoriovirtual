using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using EscritorioVirtual.Domain.Aggregates.Maps;
using MediatR;

namespace EscritorioVirtual.Application.Maps.Commands.UpdateMapLayout;

public record UpdateMapLayoutCommand(
    Guid MapId,
    List<MapObjectDto> Objects,
    List<MapZoneDto> Zones) : IRequest<MapDto>;

public class UpdateMapLayoutCommandValidator : AbstractValidator<UpdateMapLayoutCommand>
{
    public UpdateMapLayoutCommandValidator()
    {
        RuleFor(x => x.MapId).NotEmpty().WithMessage("O ID do mapa é obrigatório.");
        RuleForEach(x => x.Objects).ChildRules(obj =>
        {
            obj.RuleFor(o => o.AssetId).NotEmpty().WithMessage("O AssetId do objeto é obrigatório.");
            obj.RuleFor(o => o.CoordX).GreaterThanOrEqualTo(0).WithMessage("CoordX inválido.");
            obj.RuleFor(o => o.CoordY).GreaterThanOrEqualTo(0).WithMessage("CoordY inválido.");
        });
        RuleForEach(x => x.Zones).ChildRules(zone =>
        {
            zone.RuleFor(z => z.Name).NotEmpty().WithMessage("O nome da zona é obrigatório.");
            zone.RuleFor(z => z.StartX).GreaterThanOrEqualTo(0);
            zone.RuleFor(z => z.StartY).GreaterThanOrEqualTo(0);
            zone.RuleFor(z => z.EndX).GreaterThanOrEqualTo(z => z.StartX).WithMessage("EndX deve ser >= StartX");
            zone.RuleFor(z => z.EndY).GreaterThanOrEqualTo(z => z.StartY).WithMessage("EndY deve ser >= StartY");
        });
    }
}

public class UpdateMapLayoutCommandHandler(IMapRepository mapRepository)
    : IRequestHandler<UpdateMapLayoutCommand, MapDto>
{
    public async Task<MapDto> Handle(UpdateMapLayoutCommand request, CancellationToken cancellationToken)
    {
        var map = await mapRepository.GetMapWithDetailsAsync(request.MapId, cancellationToken);
        if (map is null)
        {
            throw new NotFoundException("Mapa não encontrado.");
        }

        var newObjects = request.Objects.Select(o => new MapObject(
            map.Id,
            o.AssetId,
            o.CoordX,
            o.CoordY,
            o.Rotation,
            o.IsSolid,
            o.ZIndexOffset,
            o.Id
        )).ToList();

        var newZones = request.Zones.Select(z => new MapZone(
            map.Id,
            z.ZoneType,
            z.Name,
            z.StartX,
            z.StartY,
            z.EndX,
            z.EndY,
            z.Capacity,
            z.Id
        )).ToList();

        map.UpdateLayout(newObjects, newZones);
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
