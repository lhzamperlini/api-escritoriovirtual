using EscritorioVirtual.Domain.Aggregates.Presence;
using EscritorioVirtual.Infrastructure.Services;
using FluentAssertions;

namespace EscritorioVirtual.UnitTests.Presence;

public class PresenceServiceTests
{
    private readonly PresenceService _presenceService = new();

    [Fact]
    public async Task AddOrUpdatePresenceAsync_ShouldStoreAndRetrievePresence()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mapId = Guid.NewGuid();
        var presence = new UserPresence
        {
            UserId = userId,
            FullName = "John Doe",
            MapId = mapId,
            X = 100,
            Y = 200,
            GridX = 3,
            GridY = 6,
            Direction = "down",
            Status = "available"
        };

        // Act
        await _presenceService.AddOrUpdatePresenceAsync(presence);
        var retrieved = await _presenceService.GetUserPresenceAsync(userId);
        var mapPresences = await _presenceService.GetMapPresencesAsync(mapId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.FullName.Should().Be("John Doe");
        retrieved.GridX.Should().Be(3);
        mapPresences.Should().ContainSingle(p => p.UserId == userId);
    }

    [Fact]
    public async Task UpdateMovementAsync_ShouldUpdateCoordinatesAndDirection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mapId = Guid.NewGuid();
        var presence = new UserPresence
        {
            UserId = userId,
            MapId = mapId,
            X = 0,
            Y = 0,
            GridX = 0,
            GridY = 0,
            Direction = "down"
        };
        await _presenceService.AddOrUpdatePresenceAsync(presence);

        // Act
        await _presenceService.UpdateMovementAsync(userId, mapId, 64, 96, 2, 3, "right", true);
        var retrieved = await _presenceService.GetUserPresenceAsync(userId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.X.Should().Be(64);
        retrieved.Y.Should().Be(96);
        retrieved.GridX.Should().Be(2);
        retrieved.GridY.Should().Be(3);
        retrieved.Direction.Should().Be("right");
        retrieved.IsMoving.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var presence = new UserPresence
        {
            UserId = userId,
            Status = "available"
        };
        await _presenceService.AddOrUpdatePresenceAsync(presence);

        // Act
        await _presenceService.UpdateStatusAsync(userId, "focus");
        var retrieved = await _presenceService.GetUserPresenceAsync(userId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Status.Should().Be("focus");
    }

    [Fact]
    public async Task UpdateZoneAsync_ShouldUpdateZoneId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var presence = new UserPresence
        {
            UserId = userId
        };
        await _presenceService.AddOrUpdatePresenceAsync(presence);

        // Act
        await _presenceService.UpdateZoneAsync(userId, zoneId);
        var retrieved = await _presenceService.GetUserPresenceAsync(userId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.CurrentZoneId.Should().Be(zoneId);
    }

    [Fact]
    public async Task RemovePresenceAsync_ShouldRemoveUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var presence = new UserPresence { UserId = userId };
        await _presenceService.AddOrUpdatePresenceAsync(presence);

        // Act
        await _presenceService.RemovePresenceAsync(userId);
        var retrieved = await _presenceService.GetUserPresenceAsync(userId);

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task GetStalePresencesAsync_ShouldDetectExpiredPresences()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var stalePresence = new UserPresence
        {
            UserId = userId1,
            LastHeartbeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 30
        };
        var freshPresence = new UserPresence
        {
            UserId = userId2,
            LastHeartbeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        await _presenceService.AddOrUpdatePresenceAsync(stalePresence);
        await _presenceService.AddOrUpdatePresenceAsync(freshPresence);

        // Act
        var staleList = await _presenceService.GetStalePresencesAsync(timeoutSeconds: 15);

        // Assert
        staleList.Should().Contain(p => p.UserId == userId1);
        staleList.Should().NotContain(p => p.UserId == userId2);
    }
}
