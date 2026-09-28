using System.IdentityModel.Tokens.Jwt;
using EscritorioVirtual.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace EscritorioVirtual.UnitTests.WebRtc;

public class LiveKitTokenServiceTests
{
    private readonly LiveKitTokenService _service;

    public LiveKitTokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "LIVEKIT_API_KEY", "test_key" },
            { "LIVEKIT_API_SECRET", "12345678901234567890123456789012" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new LiveKitTokenService(configuration);
    }

    [Fact]
    public void ResolveRoomName_ShouldReturnDeskRoom_WhenDeskZone()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        // Act
        var room = _service.ResolveRoomName(mapId, zoneId, "Desk");

        // Assert
        room.Should().Be($"desk_{zoneId}");
    }

    [Fact]
    public void ResolveRoomName_ShouldReturnMeetingRoom_WhenMeetingRoomZone()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        // Act
        var room = _service.ResolveRoomName(mapId, zoneId, "MeetingRoom");

        // Assert
        room.Should().Be($"room_{zoneId}");
    }

    [Fact]
    public void ResolveRoomName_ShouldReturnProximityMapRoom_WhenNoZone()
    {
        // Arrange
        var mapId = Guid.NewGuid();

        // Act
        var room = _service.ResolveRoomName(mapId, null, null);

        // Assert
        room.Should().Be($"proximity_{mapId}");
    }

    [Fact]
    public void GenerateToken_ShouldCreateValidJwtWithClaims()
    {
        // Arrange
        var identity = "user-123";
        var name = "Alice";
        var roomName = "proximity_map_1";

        // Act
        var token = _service.GenerateToken(identity, name, roomName, canPublish: true, canSubscribe: true);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Subject.Should().Be(identity);
        jwt.Issuer.Should().Be("test_key");
        jwt.Claims.Should().Contain(c => c.Type == "video" && c.Value.Contains("proximity_map_1"));
    }
}
