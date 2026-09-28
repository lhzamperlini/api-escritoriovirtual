using EscritorioVirtual.Domain.Aggregates.Presence;
using FluentAssertions;

namespace EscritorioVirtual.UnitTests.Presence;

public class UserPresenceTests
{
    [Fact]
    public void IsGracePeriodExpired_ShouldReturnTrue_WhenLastHeartbeatExceedsTimeout()
    {
        // Arrange
        var presence = new UserPresence
        {
            UserId = Guid.NewGuid(),
            LastHeartbeat = 1000
        };

        // Act
        var isExpired = presence.IsGracePeriodExpired(1020, timeoutSeconds: 15);

        // Assert
        isExpired.Should().BeTrue();
    }

    [Fact]
    public void IsGracePeriodExpired_ShouldReturnFalse_WhenWithinTimeout()
    {
        // Arrange
        var presence = new UserPresence
        {
            UserId = Guid.NewGuid(),
            LastHeartbeat = 1000
        };

        // Act
        var isExpired = presence.IsGracePeriodExpired(1010, timeoutSeconds: 15);

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void TouchHeartbeat_ShouldUpdateTimestampAndResetReconnectingStatus()
    {
        // Arrange
        var presence = new UserPresence
        {
            UserId = Guid.NewGuid(),
            LastHeartbeat = 1000,
            Status = "reconnecting"
        };

        // Act
        presence.TouchHeartbeat();

        // Assert
        presence.LastHeartbeat.Should().BeGreaterThan(1000);
        presence.Status.Should().Be("available");
    }

    [Fact]
    public void TouchHeartbeat_ShouldKeepCustomStatus_IfNotReconnecting()
    {
        // Arrange
        var presence = new UserPresence
        {
            UserId = Guid.NewGuid(),
            Status = "busy"
        };

        // Act
        presence.TouchHeartbeat();

        // Assert
        presence.Status.Should().Be("busy");
    }
}
