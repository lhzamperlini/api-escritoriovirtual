namespace EscritorioVirtual.Application.Common.Interfaces.Services;

public interface ILiveKitTokenService
{
    string GenerateToken(string identity, string name, string roomName, bool canPublish = true, bool canSubscribe = true);
    string ResolveRoomName(Guid mapId, Guid? zoneId, string? zoneType);
}
