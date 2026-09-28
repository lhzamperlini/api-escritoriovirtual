using EscritorioVirtual.Domain.Aggregates.Avatars;
using FluentAssertions;
using Xunit;

namespace EscritorioVirtual.UnitTests.Avatars;

public class AvatarCustomizationTests
{
    [Fact]
    public void CreateDefault_ShouldReturnExpectedDefaultParts()
    {
        // Act
        var avatar = AvatarCustomization.CreateDefault();

        // Assert
        avatar.Should().NotBeNull();
        avatar.Base.AssetId.Should().Be("skin_01");
        avatar.Base.Tint.Should().Be("#ffdbac");
        avatar.Hair.AssetId.Should().Be("hair_short_wavy");
        avatar.Hair.Tint.Should().Be("#4a3728");
        avatar.Eyes.AssetId.Should().Be("eyes_default");
        avatar.Top.AssetId.Should().Be("hoodie_classic");
        avatar.Bottom.AssetId.Should().Be("jeans_straight");
        avatar.Shoes.AssetId.Should().Be("sneakers_sport");
        avatar.Accessories.Should().ContainSingle();
        avatar.Accessories[0].AssetId.Should().Be("glasses_square");
    }

    [Fact]
    public void ToJson_And_FromJson_ShouldRoundTripCorrectly()
    {
        // Arrange
        var avatar = new AvatarCustomization
        {
            Base = new AvatarPartConfig("skin_02", "#e0ac69"),
            Hair = new AvatarPartConfig("hair_long", "#000000"),
            Eyes = new AvatarPartConfig("eyes_blue", "#0055ff"),
            Top = new AvatarPartConfig("shirt_business", "#ffffff"),
            Bottom = new AvatarPartConfig("pants_black", "#111111"),
            Shoes = new AvatarPartConfig("shoes_formal", "#222222"),
            Accessories = [new AvatarPartConfig("watch_gold", "#ffd700")]
        };

        // Act
        var json = avatar.ToJson();
        var deserialized = AvatarCustomization.FromJson(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Base.AssetId.Should().Be("skin_02");
        deserialized.Base.Tint.Should().Be("#e0ac69");
        deserialized.Hair.AssetId.Should().Be("hair_long");
        deserialized.Eyes.Tint.Should().Be("#0055ff");
        deserialized.Accessories.Should().ContainSingle();
        deserialized.Accessories[0].AssetId.Should().Be("watch_gold");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-json{]")]
    public void FromJson_WhenInvalidOrEmpty_ShouldFallbackToDefault(string? invalidJson)
    {
        // Act
        var result = AvatarCustomization.FromJson(invalidJson);

        // Assert
        result.Should().NotBeNull();
        result.Base.AssetId.Should().Be("skin_01");
    }
}
