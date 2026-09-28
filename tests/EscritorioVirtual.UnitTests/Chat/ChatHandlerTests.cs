using System.Linq.Expressions;
using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Chat.Commands;
using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Application.Chat.Queries;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Domain.AggregateRoot.Chat;
using EscritorioVirtual.Domain.Aggregates.Chat;
using FluentAssertions;
using Moq;

namespace EscritorioVirtual.UnitTests.Chat;

public class ChatHandlerTests
{
    private readonly Mock<IChatRepository> _chatRepoMock = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

    public ChatHandlerTests()
    {
        _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task SendMessageCommandHandler_ShouldAddMessageAndReturnDto()
    {
        // Arrange
        var channelId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channel = new ChatChannel(workspaceId, "Global", "#geral", id: channelId);

        _chatRepoMock.Setup(r => r.GetChannelByIdAsync(channelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel);

        var usuario = new Usuario(Guid.NewGuid(), "alice@example.com", "Alice");
        _usuarioRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var handler = new SendMessageCommandHandler(_chatRepoMock.Object, _usuarioRepoMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await handler.Handle(new SendMessageCommand(channelId, "Bem-vindo ao escritório virtual!"), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ChannelId.Should().Be(channelId);
        result.Content.Should().Be("Bem-vindo ao escritório virtual!");
        _chatRepoMock.Verify(r => r.AddMessageAsync(It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageCommandHandler_ShouldThrow_WhenChannelNotFound()
    {
        // Arrange
        var channelId = Guid.NewGuid();
        _chatRepoMock.Setup(r => r.GetChannelByIdAsync(channelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChatChannel?)null);

        var handler = new SendMessageCommandHandler(_chatRepoMock.Object, _usuarioRepoMock.Object, _currentUserServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new SendMessageCommand(channelId, "Teste"), CancellationToken.None));
    }

    [Fact]
    public async Task GetChannelsQueryHandler_ShouldReturnChannelsList()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var channelList = new List<ChatChannel>
        {
            new(workspaceId, "Global", "#geral"),
            new(workspaceId, "Zone", "#reuniao-1")
        };

        _chatRepoMock.Setup(r => r.GetOrCreateGlobalChannelAsync(workspaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(channelList[0]);

        _chatRepoMock.Setup(r => r.GetWorkspaceChannelsAsync(workspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channelList);

        var handler = new GetChannelsQueryHandler(_chatRepoMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await handler.Handle(new GetChannelsQuery(workspaceId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("#geral");
        result[1].Name.Should().Be("#reuniao-1");
    }

    [Fact]
    public async Task CreateDirectChannelCommandHandler_ShouldReturnDirectChannelDto()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var targetUser = new Usuario(targetUserId, "bob@example.com", "Bob");

        _usuarioRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);

        var channel = new ChatChannel(workspaceId, "Direct", "Bob");
        _chatRepoMock.Setup(r => r.GetOrCreateDirectChannelAsync(
            workspaceId, It.IsAny<Guid>(), targetUserId, "Bob", It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel);

        var handler = new CreateDirectChannelCommandHandler(_chatRepoMock.Object, _usuarioRepoMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await handler.Handle(new CreateDirectChannelCommand(workspaceId, targetUserId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ChannelType.Should().Be("Direct");
        result.Name.Should().Be("Bob");
    }
}
