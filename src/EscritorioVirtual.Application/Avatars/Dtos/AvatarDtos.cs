namespace EscritorioVirtual.Application.Avatars.Dtos;

public record AvatarPartDto
{
    public string AssetId { get; init; } = string.Empty;
    public string Tint { get; init; } = "#ffffff";

    public AvatarPartDto() { }

    public AvatarPartDto(string assetId, string tint)
    {
        AssetId = assetId;
        Tint = tint;
    }
}

public record AvatarConfigDto
{
    public AvatarPartDto Base { get; init; } = new("skin_01", "#ffdbac");
    public AvatarPartDto Hair { get; init; } = new("hair_short_wavy", "#4a3728");
    public AvatarPartDto Eyes { get; init; } = new("eyes_default", "#2e536f");
    public AvatarPartDto Top { get; init; } = new("hoodie_classic", "#1e3a8a");
    public AvatarPartDto Bottom { get; init; } = new("jeans_straight", "#1f2937");
    public AvatarPartDto Shoes { get; init; } = new("sneakers_sport", "#ffffff");
    public List<AvatarPartDto> Accessories { get; init; } = [new("glasses_square", "#111827")];
}
