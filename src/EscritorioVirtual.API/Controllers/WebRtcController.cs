using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EscritorioVirtual.API.Controllers;

public class WebRtcController(
    ILiveKitTokenService liveKitTokenService,
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService,
    IConfiguration configuration) : ApiControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<WebRtcTokenResponse>> GetToken([FromBody] WebRtcTokenRequest request)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var usuario = await usuarioRepository.GetAsync(u => u.Id == userId);
        var fullName = usuario?.FullName ?? "Colega";
        var identity = userId.ToString();

        var roomName = liveKitTokenService.ResolveRoomName(request.MapId, request.ZoneId, request.ZoneType);
        var token = liveKitTokenService.GenerateToken(identity, fullName, roomName, request.CanPublish ?? true, request.CanSubscribe ?? true);
        var liveKitUrl = configuration["LIVEKIT_URL"] ?? "ws://localhost:7880";

        return Ok(new WebRtcTokenResponse(token, roomName, liveKitUrl, identity, fullName));
    }
}

public record WebRtcTokenRequest(Guid WorkspaceId, Guid MapId, Guid? ZoneId, string? ZoneType, bool? CanPublish, bool? CanSubscribe);
public record WebRtcTokenResponse(string Token, string RoomName, string LiveKitUrl, string Identity, string FullName);
