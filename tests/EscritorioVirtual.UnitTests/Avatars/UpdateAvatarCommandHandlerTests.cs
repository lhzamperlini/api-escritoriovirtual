using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Avatars.Commands.UpdateAvatar;
using EscritorioVirtual.Application.Avatars.Dtos;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.AggregateRoot;
using FluentAssertions;
using Moq;
using Xunit;

namespace EscritorioVirtual.UnitTests.Avatars;

public class UpdateAvatarCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UpdateAvatarCommandHandler _handler;

    public UpdateAvatarCommandHandlerTests()
    {
        _handler = new UpdateAvatarCommandHandler(_usuarioRepositoryMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.UserId).Returns((Guid?)null);
        var command = new UpdateAvatarCommand(new AvatarConfigDto());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserNotFoundInRepository_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioAutenticadoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var command = new UpdateAvatarCommand(new AvatarConfigDto());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpdateUserAndReturnDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Usuario(userId, "test@test.com", "Test User");

        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioAutenticadoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var avatarDto = new AvatarConfigDto
        {
            Base = new AvatarPartDto("skin_03", "#d09b62"),
            Hair = new AvatarPartDto("hair_curly", "#221100"),
            Eyes = new AvatarPartDto("eyes_green", "#00aa55"),
            Top = new AvatarPartDto("jacket_leather", "#332211"),
            Bottom = new AvatarPartDto("pants_cargo", "#556644"),
            Shoes = new AvatarPartDto("boots_combat", "#111111"),
            Accessories = [new AvatarPartDto("earring_silver", "#cccccc")]
        };

        var command = new UpdateAvatarCommand(avatarDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(avatarDto);
        user.AvatarConfig.Should().Contain("skin_03");
        user.AvatarConfig.Should().Contain("#d09b62");
        _usuarioRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
