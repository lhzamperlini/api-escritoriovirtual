using System.Text.Json;
using System.Text.Json.Serialization;

namespace EscritorioVirtual.Domain.Aggregates.Avatars;

public record AvatarPartConfig
{
    [JsonPropertyName("assetId")]
    public string AssetId { get; init; } = string.Empty;

    [JsonPropertyName("tint")]
    public string Tint { get; init; } = "#ffffff";

    public AvatarPartConfig() { }

    public AvatarPartConfig(string assetId, string tint)
    {
        AssetId = assetId;
        Tint = tint;
    }
}

public class AvatarCustomization
{
    [JsonPropertyName("base")]
    public AvatarPartConfig Base { get; set; } = new("skin_01", "#ffdbac");

    [JsonPropertyName("hair")]
    public AvatarPartConfig Hair { get; set; } = new("hair_short_wavy", "#4a3728");

    [JsonPropertyName("eyes")]
    public AvatarPartConfig Eyes { get; set; } = new("eyes_default", "#2e536f");

    [JsonPropertyName("top")]
    public AvatarPartConfig Top { get; set; } = new("hoodie_classic", "#1e3a8a");

    [JsonPropertyName("bottom")]
    public AvatarPartConfig Bottom { get; set; } = new("jeans_straight", "#1f2937");

    [JsonPropertyName("shoes")]
    public AvatarPartConfig Shoes { get; set; } = new("sneakers_sport", "#ffffff");

    [JsonPropertyName("accessories")]
    public List<AvatarPartConfig> Accessories { get; set; } = [new("glasses_square", "#111827")];

    public static AvatarCustomization CreateDefault() => new();

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    public static AvatarCustomization FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return CreateDefault();

        try
        {
            var result = JsonSerializer.Deserialize<AvatarCustomization>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result ?? CreateDefault();
        }
        catch
        {
            return CreateDefault();
        }
    }
}
