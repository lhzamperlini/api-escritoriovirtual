using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Maps.Commands.CreateMap;
using EscritorioVirtual.Application.Maps.Interfaces;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Maps;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EscritorioVirtual.UnitTests.Maps;

public class CreateMapCommandValidatorTests
{
    private readonly CreateMapCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateMapCommand(Guid.NewGuid(), "Mapa Alpha", 100, 100, 32);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenEmptyName_ShouldFail(string? name)
    {
        // Arrange
        var command = new CreateMapCommand(Guid.NewGuid(), name!, 100, 100, 32);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_WhenInvalidDimensions_ShouldFail(int dim)
    {
        // Arrange
        var command = new CreateMapCommand(Guid.NewGuid(), "Mapa", dim, 100, 32);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "GridWidth");
    }
}

public class CreateMapCommandHandlerTests
{
    private readonly Mock<IMapRepository> _mapRepoMock = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly CreateMapCommandHandler _handler;

    public CreateMapCommandHandlerTests()
    {
        _handler = new CreateMapCommandHandler(_mapRepoMock.Object, _workspaceRepoMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldThrowUnauthorized()
    {
        // Arrange
        _currentUserMock.Setup(s => s.UserId).Returns((Guid?)null);
        var command = new CreateMapCommand(Guid.NewGuid(), "Mapa");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenWorkspaceNotFound_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wsId = Guid.NewGuid();
        _currentUserMock.Setup(s => s.UserId).Returns(userId);
        _workspaceRepoMock.Setup(w => w.GetByIdWithMembersAsync(wsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workspace?)null);

        var command = new CreateMapCommand(wsId, "Mapa");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateMapWithInitialZones()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wsId = Guid.NewGuid();
        var workspace = new Workspace("Minha Empresa", "minha-empresa", userId);

        _currentUserMock.Setup(s => s.UserId).Returns(userId);
        _workspaceRepoMock.Setup(w => w.GetByIdWithMembersAsync(wsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workspace);

        var command = new CreateMapCommand(wsId, "Andar 01", 100, 100, 32);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Andar 01");
        result.WorkspaceId.Should().Be(wsId);
        result.Zones.Should().NotBeEmpty();
        result.Zones.Should().Contain(z => z.ZoneType == EscritorioVirtual.Domain.Enums.ZoneType.Spawn);
        result.Objects.Should().NotBeEmpty();

        _mapRepoMock.Verify(m => m.AddMapAsync(It.IsAny<Map>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
