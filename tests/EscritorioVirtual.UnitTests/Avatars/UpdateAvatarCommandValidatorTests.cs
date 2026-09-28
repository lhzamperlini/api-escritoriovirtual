using EscritorioVirtual.Application.Avatars.Commands.UpdateAvatar;
using EscritorioVirtual.Application.Avatars.Dtos;
using FluentAssertions;
using Xunit;

namespace EscritorioVirtual.UnitTests.Avatars;

public class UpdateAvatarCommandValidatorTests
{
    private readonly UpdateAvatarCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidAvatarConfig_ShouldPass()
    {
        // Arrange
        var command = new UpdateAvatarCommand(new AvatarConfigDto
        {
            Base = new AvatarPartDto("skin_01", "#ffdbac"),
            Hair = new AvatarPartDto("hair_short", "#4a3728"),
            Eyes = new AvatarPartDto("eyes_default", "#2e536f"),
            Top = new AvatarPartDto("shirt_blue", "#1e3a8a"),
            Bottom = new AvatarPartDto("pants_black", "#1f2937"),
            Shoes = new AvatarPartDto("shoes_white", "#ffffff"),
            Accessories = [new AvatarPartDto("hat_cap", "#111827")]
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenNullConfig_ShouldFail()
    {
        // Arrange
        var command = new UpdateAvatarCommand(null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AvatarConfig");
    }

    [Theory]
    [InlineData("invalid-hex")]
    [InlineData("#12")]
    [InlineData("#12345")]
    [InlineData("#GGGGGG")]
    [InlineData("")]
    public void Validate_WhenInvalidHexColor_ShouldFail(string invalidColor)
    {
        // Arrange
        var command = new UpdateAvatarCommand(new AvatarConfigDto
        {
            Base = new AvatarPartDto("skin_01", invalidColor),
            Hair = new AvatarPartDto("hair_short", "#4a3728"),
            Eyes = new AvatarPartDto("eyes_default", "#2e536f"),
            Top = new AvatarPartDto("shirt_blue", "#1e3a8a"),
            Bottom = new AvatarPartDto("pants_black", "#1f2937"),
            Shoes = new AvatarPartDto("shoes_white", "#ffffff")
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Base.Tint"));
    }

    [Fact]
    public void Validate_WhenEmptyAssetId_ShouldFail()
    {
        // Arrange
        var command = new UpdateAvatarCommand(new AvatarConfigDto
        {
            Base = new AvatarPartDto("", "#ffdbac"),
            Hair = new AvatarPartDto("hair_short", "#4a3728"),
            Eyes = new AvatarPartDto("eyes_default", "#2e536f"),
            Top = new AvatarPartDto("shirt_blue", "#1e3a8a"),
            Bottom = new AvatarPartDto("pants_black", "#1f2937"),
            Shoes = new AvatarPartDto("shoes_white", "#ffffff")
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Base.AssetId"));
    }
}
