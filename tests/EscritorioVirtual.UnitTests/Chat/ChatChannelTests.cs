using EscritorioVirtual.Domain.AggregateRoot.Chat;
using FluentAssertions;

namespace EscritorioVirtual.UnitTests.Chat;

public class ChatChannelTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        // Act
        var channel = new ChatChannel(workspaceId, "Zone", "#mesa-squad-alpha", zoneId);

        // Assert
        channel.WorkspaceId.Should().Be(workspaceId);
        channel.ChannelType.Should().Be("Zone");
        channel.Name.Should().Be("#mesa-squad-alpha");
        channel.ZoneId.Should().Be(zoneId);
        channel.Members.Should().BeEmpty();
        channel.Messages.Should().BeEmpty();
    }

    [Fact]
    public void AddMember_ShouldAddMemberOnce()
    {
        // Arrange
        var channel = new ChatChannel(Guid.NewGuid(), "Global", "#geral");
        var userId = Guid.NewGuid();

        // Act
        channel.AddMember(userId);
        channel.AddMember(userId); // duplicate add

        // Assert
        channel.Members.Should().HaveCount(1);
        channel.Members.First().UserId.Should().Be(userId);
    }

    [Fact]
    public void AddMessage_ShouldAddMessageToCollection()
    {
        // Arrange
        var channel = new ChatChannel(Guid.NewGuid(), "Global", "#geral");
        var senderId = Guid.NewGuid();

        // Act
        var msg = channel.AddMessage(senderId, "Alice", "Olá equipe!");

        // Assert
        channel.Messages.Should().HaveCount(1);
        msg.SenderName.Should().Be("Alice");
        msg.Content.Should().Be("Olá equipe!");
        msg.IsRead.Should().BeFalse();
    }

    [Fact]
    public void MarkMemberRead_ShouldUpdateLastReadTimestamp()
    {
        // Arrange
        var channel = new ChatChannel(Guid.NewGuid(), "Direct", "Alice");
        var userId = Guid.NewGuid();
        channel.AddMember(userId);

        var member = channel.Members.First();
        var initialRead = member.LastReadAt;

        // Act
        channel.MarkMemberRead(userId);

        // Assert
        member.LastReadAt.Should().BeOnOrAfter(initialRead);
    }
}
