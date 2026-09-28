using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Maps;
using EscritorioVirtual.Domain.Aggregates.Maps;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Maps.Commands.CreateMap;

public record CreateMapCommand(
    Guid WorkspaceId,
    string Name,
    int GridWidth = 100,
    int GridHeight = 100,
    int TileSize = 32,
    string? TiledMapData = null) : IRequest<MapDto>;

public class CreateMapCommandValidator : AbstractValidator<CreateMapCommand>
{
    public CreateMapCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty().WithMessage("O Workspace é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome do mapa é obrigatório.").MaximumLength(100);
        RuleFor(x => x.GridWidth).GreaterThan(0).WithMessage("A largura da grade deve ser maior que 0.");
        RuleFor(x => x.GridHeight).GreaterThan(0).WithMessage("A altura da grade deve ser maior que 0.");
        RuleFor(x => x.TileSize).GreaterThan(0).WithMessage("O tamanho do bloco (tile) deve ser maior que 0.");
    }
}

public class CreateMapCommandHandler(
    IMapRepository mapRepository,
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<CreateMapCommand, MapDto>
{
    public async Task<MapDto> Handle(CreateMapCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace is null)
        {
            throw new NotFoundException("Workspace não encontrado.");
        }

        var map = new Map(
            request.WorkspaceId,
            request.Name,
            request.GridWidth,
            request.GridHeight,
            request.TileSize,
            request.TiledMapData);

        // Se for um mapa novo sem dados importados, inicializa zona de Spawn e Lounge padrão
        if (string.IsNullOrWhiteSpace(request.TiledMapData))
        {
            map.AddZone(new MapZone(map.Id, ZoneType.Spawn, "Ponto de Spawn Principal", 10, 10, 15, 15));
            map.AddZone(new MapZone(map.Id, ZoneType.Desk, "Ilha de Trabalho Alpha", 20, 15, 30, 25, 6));
            map.AddZone(new MapZone(map.Id, ZoneType.MeetingRoom, "Sala de Reunião 1", 35, 10, 50, 25, 10));
            map.AddZone(new MapZone(map.Id, ZoneType.Lounge, "Área de Convivência (Lounge)", 15, 30, 35, 45));

            map.AddObject(new MapObject(map.Id, "desk_wood_01", 22, 18, 0, true));
            map.AddObject(new MapObject(map.Id, "chair_office_01", 22, 19, 0, false));
            map.AddObject(new MapObject(map.Id, "plant_potted_01", 11, 11, 0, true));
        }

        await mapRepository.AddMapAsync(map, cancellationToken);

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
