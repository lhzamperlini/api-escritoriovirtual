using EscritorioVirtual.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace EscritorioVirtual.UnitTests.Rooms;

public class RoomAccessServiceTests
{
    private readonly RoomAccessService _service = new();

    [Fact]
    public void CanEnterRoom_WhenRoomNotLocked_ShouldReturnTrue()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var canEnter = _service.CanEnterRoom(workspaceId, zoneId, userId, isOwnerOrAdmin: false);

        // Assert
        canEnter.Should().BeTrue();
    }

    [Fact]
    public void CanEnterRoom_WhenRoomLocked_AndUserNotApproved_ShouldReturnFalse()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();

        _service.SetRoomLock(zoneId, true, hostId);

        // Act
        var canEnter = _service.CanEnterRoom(workspaceId, zoneId, visitorId, isOwnerOrAdmin: false);

        // Assert
        canEnter.Should().BeFalse();
        _service.IsRoomLocked(zoneId).Should().BeTrue();
        _service.GetLockedBy(zoneId).Should().Be(hostId);
    }

    [Fact]
    public void CanEnterRoom_WhenRoomLocked_AndUserIsHost_ShouldReturnTrue()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var hostId = Guid.NewGuid();

        _service.SetRoomLock(zoneId, true, hostId);

        // Act
        var canEnter = _service.CanEnterRoom(workspaceId, zoneId, hostId, isOwnerOrAdmin: false);

        // Assert
        canEnter.Should().BeTrue();
    }

    [Fact]
    public void CanEnterRoom_WhenRoomLocked_AndUserIsApprovedGuest_ShouldReturnTrue()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var guestId = Guid.NewGuid();

        _service.SetRoomLock(zoneId, true, hostId);
        _service.ApproveGuest(zoneId, guestId);

        // Act
        var canEnter = _service.CanEnterRoom(workspaceId, zoneId, guestId, isOwnerOrAdmin: false);

        // Assert
        canEnter.Should().BeTrue();
    }

    [Fact]
    public void CanEnterRoom_WhenRoomLocked_AndUserIsAdminOrOwner_ShouldReturnTrue_MasterKey()
    {
        // Arrange (US04 - Chave-Mestra)
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        _service.SetRoomLock(zoneId, true, hostId);

        // Act
        var canEnter = _service.CanEnterRoom(workspaceId, zoneId, adminId, isOwnerOrAdmin: true);

        // Assert
        canEnter.Should().BeTrue();
    }

    [Fact]
    public void SetRoomLock_WhenUnlocked_ShouldClearAllowedGuests()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var guestId = Guid.NewGuid();

        _service.SetRoomLock(zoneId, true, hostId);
        _service.ApproveGuest(zoneId, guestId);
        _service.SetRoomLock(zoneId, false, hostId);

        // Now lock again
        _service.SetRoomLock(zoneId, true, hostId);

        // Act
        var canEnterGuest = _service.CanEnterRoom(workspaceId, zoneId, guestId, isOwnerOrAdmin: false);

        // Assert
        canEnterGuest.Should().BeFalse();
    }
}
