using EscritorioVirtual.Domain.AggregateRoot;
using FluentAssertions;
using Xunit;

namespace EscritorioVirtual.UnitTests.Avatars;

public class UsuarioAvatarTests
{
    [Fact]
    public void UpdateAvatar_ShouldSetAvatarConfigAndDataAtualizacao()
    {
        // Arrange
        var user = new Usuario(Guid.NewGuid(), "dev@test.com", "Dev User");
        var avatarJson = "{\"base\":{\"assetId\":\"skin_02\",\"tint\":\"#ffffff\"}}";

        // Act
        user.UpdateAvatar(avatarJson);

        // Assert
        user.AvatarConfig.Should().Be(avatarJson);
        user.DataAtualizacao.Should().NotBeNull();
        user.DataAtualizacao.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
