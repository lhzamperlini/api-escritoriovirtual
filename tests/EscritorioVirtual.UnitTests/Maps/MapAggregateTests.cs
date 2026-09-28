using EscritorioVirtual.Domain.AggregateRoot.Maps;
using EscritorioVirtual.Domain.Aggregates.Maps;
using EscritorioVirtual.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EscritorioVirtual.UnitTests.Maps;

public class MapAggregateTests
{
    [Fact]
    public void Map_Create_ShouldInitializeWithDefaultProperties()
    {
        // Arrange
        var wsId = Guid.NewGuid();

        // Act
        var map = new Map(wsId, "Andar Principal", 120, 80, 32);

        // Assert
        map.WorkspaceId.Should().Be(wsId);
        map.Name.Should().Be("Andar Principal");
        map.GridWidth.Should().Be(120);
        map.GridHeight.Should().Be(80);
        map.TileSize.Should().Be(32);
        map.Objects.Should().BeEmpty();
        map.Zones.Should().BeEmpty();
        map.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Map_AddObject_And_RemoveObject_ShouldUpdateCollection()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Teste");
        var obj = new MapObject(map.Id, "table_wood", 10, 15, 90, true, 1);

        // Act
        map.AddObject(obj);

        // Assert
        map.Objects.Should().ContainSingle();
        map.Objects.First().AssetId.Should().Be("table_wood");

        // Act remove
        var removed = map.RemoveObject(obj.Id);

        // Assert
        removed.Should().BeTrue();
        map.Objects.Should().BeEmpty();
    }

    [Fact]
    public void Map_AddZone_And_RemoveZone_ShouldUpdateCollection()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Teste");
        var zone = new MapZone(map.Id, ZoneType.MeetingRoom, "Sala de Guerra", 10, 10, 20, 20, 8);

        // Act
        map.AddZone(zone);

        // Assert
        map.Zones.Should().ContainSingle();
        map.Zones.First().Name.Should().Be("Sala de Guerra");
        map.Zones.First().ZoneType.Should().Be(ZoneType.MeetingRoom);

        // Act remove
        var removed = map.RemoveZone(zone.Id);

        // Assert
        removed.Should().BeTrue();
        map.Zones.Should().BeEmpty();
    }

    [Fact]
    public void Map_UpdateLayout_ShouldReplaceObjectsAndZonesAtomically()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Teste");
        map.AddObject(new MapObject(map.Id, "old_obj", 1, 1));
        map.AddZone(new MapZone(map.Id, ZoneType.Lounge, "Old Lounge", 0, 0, 5, 5));

        var newObjects = new List<MapObject>
        {
            new(map.Id, "new_obj_1", 10, 10),
            new(map.Id, "new_obj_2", 20, 20)
        };
        var newZones = new List<MapZone>
        {
            new(map.Id, ZoneType.Desk, "Nova Mesa", 15, 15, 25, 25)
        };

        // Act
        map.UpdateLayout(newObjects, newZones);

        // Assert
        map.Objects.Should().HaveCount(2);
        map.Objects.Select(o => o.AssetId).Should().Contain(["new_obj_1", "new_obj_2"]);
        map.Zones.Should().ContainSingle();
        map.Zones.First().Name.Should().Be("Nova Mesa");
    }

    [Fact]
    public void Map_ImportTiledData_ShouldSaveJsonData()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Tiled");
        var tiledJson = "{\"layers\":[{\"name\":\"Ground\",\"width\":50,\"height\":50}]}";

        // Act
        map.ImportTiledData(tiledJson);

        // Assert
        map.TiledMapData.Should().Be(tiledJson);
    }
}
