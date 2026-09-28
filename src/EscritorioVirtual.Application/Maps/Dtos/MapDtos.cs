using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Maps.Dtos;

public record MapObjectDto
{
    public Guid? Id { get; init; }
    public Guid? MapId { get; init; }
    public string AssetId { get; init; } = string.Empty;
    public int CoordX { get; init; }
    public int CoordY { get; init; }
    public int Rotation { get; init; }
    public bool IsSolid { get; init; } = true;
    public int ZIndexOffset { get; init; }
}

public record MapZoneDto
{
    public Guid? Id { get; init; }
    public Guid? MapId { get; init; }
    public ZoneType ZoneType { get; init; }
    public string Name { get; init; } = string.Empty;
    public int StartX { get; init; }
    public int StartY { get; init; }
    public int EndX { get; init; }
    public int EndY { get; init; }
    public int? Capacity { get; init; }
}

public record MapDto
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int GridWidth { get; init; } = 100;
    public int GridHeight { get; init; } = 100;
    public int TileSize { get; init; } = 32;
    public string? TiledMapData { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<MapObjectDto> Objects { get; init; } = [];
    public List<MapZoneDto> Zones { get; init; } = [];
}

public record UpdateMapLayoutDto
{
    public List<MapObjectDto> Objects { get; init; } = [];
    public List<MapZoneDto> Zones { get; init; } = [];
}
