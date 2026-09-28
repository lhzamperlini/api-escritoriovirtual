using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Domain.Aggregates.Maps;

public class MapZone
{
    public Guid Id { get; private set; }
    public Guid MapId { get; private set; }
    public ZoneType ZoneType { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int StartX { get; private set; }
    public int StartY { get; private set; }
    public int EndX { get; private set; }
    public int EndY { get; private set; }
    public int? Capacity { get; private set; }

    protected MapZone() { }

    public MapZone(Guid mapId, ZoneType zoneType, string name, int startX, int startY, int endX, int endY, int? capacity = null, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        MapId = mapId;
        ZoneType = zoneType;
        Name = name.Trim();
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
        Capacity = capacity;
    }

    public void UpdateCoordinates(int startX, int startY, int endX, int endY)
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }

    public void UpdateDetails(string name, ZoneType zoneType, int? capacity)
    {
        Name = name.Trim();
        ZoneType = zoneType;
        Capacity = capacity;
    }
}
