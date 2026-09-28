using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Maps.Commands.UpdateMapLayout;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Interfaces;
using EscritorioVirtual.Application.Maps.Queries.GetMapById;
using EscritorioVirtual.Domain.AggregateRoot.Maps;
using EscritorioVirtual.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace EscritorioVirtual.UnitTests.Maps;

public class UpdateMapLayoutCommandHandlerTests
{
    private readonly Mock<IMapRepository> _mapRepoMock = new();
    private readonly UpdateMapLayoutCommandHandler _handler;

    public UpdateMapLayoutCommandHandlerTests()
    {
        _handler = new UpdateMapLayoutCommandHandler(_mapRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMapNotFound_ShouldThrowNotFound()
    {
        // Arrange
        _mapRepoMock.Setup(m => m.GetMapWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Map?)null);

        var command = new UpdateMapLayoutCommand(Guid.NewGuid(), [], []);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpdateMapLayoutAndReturnDto()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Central");
        _mapRepoMock.Setup(m => m.GetMapWithDetailsAsync(map.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(map);

        var objects = new List<MapObjectDto>
        {
            new() { AssetId = "chair_ergonomic", CoordX = 12, CoordY = 14, Rotation = 180, IsSolid = false }
        };
        var zones = new List<MapZoneDto>
        {
            new() { ZoneType = ZoneType.Desk, Name = "Mesa Suporte", StartX = 10, StartY = 10, EndX = 20, EndY = 20 }
        };

        var command = new UpdateMapLayoutCommand(map.Id, objects, zones);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Objects.Should().ContainSingle();
        result.Objects[0].AssetId.Should().Be("chair_ergonomic");
        result.Zones.Should().ContainSingle();
        result.Zones[0].Name.Should().Be("Mesa Suporte");

        _mapRepoMock.Verify(m => m.UpdateMapAsync(map, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetMapByIdQueryHandlerTests
{
    private readonly Mock<IMapRepository> _mapRepoMock = new();
    private readonly GetMapByIdQueryHandler _handler;

    public GetMapByIdQueryHandlerTests()
    {
        _handler = new GetMapByIdQueryHandler(_mapRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMapFound_ShouldReturnDetails()
    {
        // Arrange
        var map = new Map(Guid.NewGuid(), "Mapa Floor 1");
        _mapRepoMock.Setup(m => m.GetMapWithDetailsAsync(map.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(map);

        // Act
        var result = await _handler.Handle(new GetMapByIdQuery(map.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(map.Id);
        result.Name.Should().Be("Mapa Floor 1");
    }

    [Fact]
    public async Task Handle_WhenMapNotFound_ShouldThrowNotFound()
    {
        // Arrange
        _mapRepoMock.Setup(m => m.GetMapWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Map?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetMapByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
