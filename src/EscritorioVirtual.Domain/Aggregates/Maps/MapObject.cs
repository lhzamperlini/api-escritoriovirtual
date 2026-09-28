namespace EscritorioVirtual.Domain.Aggregates.Maps;

public class MapObject
{
    public Guid Id { get; private set; }
    public Guid MapId { get; private set; }
    public string AssetId { get; private set; } = string.Empty;
    public int CoordX { get; private set; }
    public int CoordY { get; private set; }
    public int Rotation { get; private set; }
    public bool IsSolid { get; private set; }
    public int ZIndexOffset { get; private set; }

    protected MapObject() { }

    public MapObject(Guid mapId, string assetId, int coordX, int coordY, int rotation = 0, bool isSolid = true, int zIndexOffset = 0, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        MapId = mapId;
        AssetId = assetId;
        CoordX = coordX;
        CoordY = coordY;
        Rotation = rotation;
        IsSolid = isSolid;
        ZIndexOffset = zIndexOffset;
    }

    public void UpdatePosition(int coordX, int coordY, int rotation)
    {
        CoordX = coordX;
        CoordY = coordY;
        Rotation = rotation;
    }

    public void UpdateProperties(bool isSolid, int zIndexOffset)
    {
        IsSolid = isSolid;
        ZIndexOffset = zIndexOffset;
    }
}
