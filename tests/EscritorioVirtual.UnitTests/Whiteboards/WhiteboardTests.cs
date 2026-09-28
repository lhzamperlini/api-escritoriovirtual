using EscritorioVirtual.Application.Whiteboards.Commands.SaveSnapshot;
using EscritorioVirtual.Application.Whiteboards.Interfaces;
using EscritorioVirtual.Application.Whiteboards.Queries.GetWhiteboard;
using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;
using FluentAssertions;
using Moq;
using Xunit;

namespace EscritorioVirtual.UnitTests.Whiteboards;

public class WhiteboardTests
{
    [Fact]
    public void Whiteboard_WhenCreated_ShouldInitializeDefaults()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        // Act
        var whiteboard = new Whiteboard(workspaceId, "Brainstorming Sprint 1", zoneId);

        // Assert
        whiteboard.Id.Should().NotBeEmpty();
        whiteboard.WorkspaceId.Should().Be(workspaceId);
        whiteboard.ZoneId.Should().Be(zoneId);
        whiteboard.Name.Should().Be("Brainstorming Sprint 1");
        whiteboard.DocumentData.Should().Be("{}");
        whiteboard.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Whiteboard_UpdateSnapshot_ShouldUpdateDataAndTimestamp()
    {
        // Arrange
        var whiteboard = new Whiteboard(Guid.NewGuid(), "Design Architecture");
        var sampleJson = "{\"shapes\": [{\"id\": 1, \"type\": \"box\"}]}";

        // Act
        whiteboard.UpdateSnapshot(sampleJson);

        // Assert
        whiteboard.DocumentData.Should().Be(sampleJson);
        whiteboard.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task SaveWhiteboardSnapshotCommandHandler_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var mockRepo = new Mock<IWhiteboardRepository>();
        var whiteboard = new Whiteboard(Guid.NewGuid(), "Retro Room");
        mockRepo.Setup(r => r.GetByIdAsync(whiteboard.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(whiteboard);

        var handler = new SaveWhiteboardSnapshotCommandHandler(mockRepo.Object);
        var command = new SaveWhiteboardSnapshotCommand(whiteboard.Id, "{\"lines\": []}");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DocumentData.Should().Be("{\"lines\": []}");
        mockRepo.Verify(r => r.UpdateAsync(whiteboard, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetWhiteboardQueryHandler_WhenZoneBoardDoesNotExist_ShouldAutoCreate()
    {
        // Arrange
        var mockRepo = new Mock<IWhiteboardRepository>();
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        mockRepo.Setup(r => r.GetByZoneIdAsync(workspaceId, zoneId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Whiteboard?)null);

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Whiteboard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Whiteboard b, CancellationToken _) => b);

        var handler = new GetWhiteboardQueryHandler(mockRepo.Object);
        var query = new GetWhiteboardQuery(null, workspaceId, zoneId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.WorkspaceId.Should().Be(workspaceId);
        result.ZoneId.Should().Be(zoneId);
        result.DocumentData.Should().Be("{}");
        mockRepo.Verify(r => r.CreateAsync(It.IsAny<Whiteboard>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
