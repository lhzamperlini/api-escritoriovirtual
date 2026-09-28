using EscritorioVirtual.Domain.Aggregates.Maps;

namespace EscritorioVirtual.Domain.AggregateRoot.Maps;

public class Map : BaseAggregateRoot<Guid>
{
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int GridWidth { get; private set; } = 100;
    public int GridHeight { get; private set; } = 100;
    public int TileSize { get; private set; } = 32;
    public string? TiledMapData { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MapObject> _objects = new();
    public virtual IReadOnlyCollection<MapObject> Objects => _objects.AsReadOnly();

    private readonly List<MapZone> _zones = new();
    public virtual IReadOnlyCollection<MapZone> Zones => _zones.AsReadOnly();

    protected Map() { }

    public Map(Guid workspaceId, string name, int gridWidth = 100, int gridHeight = 100, int tileSize = 32, string? tiledMapData = null, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        WorkspaceId = workspaceId;
        Name = name.Trim();
        GridWidth = gridWidth > 0 ? gridWidth : 100;
        GridHeight = gridHeight > 0 ? gridHeight : 100;
        TileSize = tileSize > 0 ? tileSize : 32;
        TiledMapData = tiledMapData;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DataCriacao = DateTime.UtcNow;
        Status = true;
    }

    public void UpdateDetails(string name, int gridWidth, int gridHeight, int tileSize)
    {
        Name = name.Trim();
        GridWidth = gridWidth > 0 ? gridWidth : GridWidth;
        GridHeight = gridHeight > 0 ? gridHeight : GridHeight;
        TileSize = tileSize > 0 ? tileSize : TileSize;
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void ImportTiledData(string tiledJson)
    {
        TiledMapData = tiledJson;
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AddObject(MapObject obj)
    {
        _objects.Add(obj);
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool RemoveObject(Guid objectId)
    {
        var obj = _objects.FirstOrDefault(o => o.Id == objectId);
        if (obj is null) return false;
        var removed = _objects.Remove(obj);
        if (removed)
        {
            UpdatedAt = DateTime.UtcNow;
            DataAtualizacao = DateTime.UtcNow;
        }
        return removed;
    }

    public void ClearObjects()
    {
        _objects.Clear();
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AddZone(MapZone zone)
    {
        _zones.Add(zone);
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool RemoveZone(Guid zoneId)
    {
        var zone = _zones.FirstOrDefault(z => z.Id == zoneId);
        if (zone is null) return false;
        var removed = _zones.Remove(zone);
        if (removed)
        {
            UpdatedAt = DateTime.UtcNow;
            DataAtualizacao = DateTime.UtcNow;
        }
        return removed;
    }

    public void ClearZones()
    {
        _zones.Clear();
        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void UpdateLayout(IEnumerable<MapObject> newObjects, IEnumerable<MapZone> newZones)
    {
        _objects.Clear();
        _objects.AddRange(newObjects);

        _zones.Clear();
        _zones.AddRange(newZones);

        UpdatedAt = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }
}
