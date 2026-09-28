using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EscritorioVirtual.Infrastructure.Services;

public class LiveKitTokenService(IConfiguration configuration) : ILiveKitTokenService
{
    private readonly string _apiKey = configuration["LIVEKIT_API_KEY"] ?? "devkey";
    private readonly string _apiSecret = configuration["LIVEKIT_API_SECRET"] ?? "secret_key_escritorio_virtual_webrtc_2026";

    public string ResolveRoomName(Guid mapId, Guid? zoneId, string? zoneType)
    {
        if (zoneId.HasValue && !string.IsNullOrWhiteSpace(zoneType))
        {
            if (string.Equals(zoneType, "Desk", StringComparison.OrdinalIgnoreCase))
            {
                return $"desk_{zoneId.Value}";
            }
            if (string.Equals(zoneType, "MeetingRoom", StringComparison.OrdinalIgnoreCase))
            {
                return $"room_{zoneId.Value}";
            }
        }

        return $"proximity_{mapId}";
    }

    public string GenerateToken(string identity, string name, string roomName, bool canPublish = true, bool canSubscribe = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_apiSecret.PadRight(32));

        var videoGrants = new Dictionary<string, object>
        {
            { "room", roomName },
            { "roomJoin", true },
            { "canPublish", canPublish },
            { "canSubscribe", canSubscribe }
        };

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identity),
            new(JwtRegisteredClaimNames.Name, name),
            new(JwtRegisteredClaimNames.Iss, _apiKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("video", JsonSerializer.Serialize(videoGrants), JsonClaimValueTypes.Json)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(6),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
