using EscritorioVirtual.Domain.Aggregates.Presence;

namespace EscritorioVirtual.Application.Common.Interfaces.Services;

public interface IPresenceService
{
    Task AddOrUpdatePresenceAsync(UserPresence presence, CancellationToken cancellationToken = default);
    Task<UserPresence?> GetUserPresenceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserPresence>> GetMapPresencesAsync(Guid mapId, CancellationToken cancellationToken = default);
    Task UpdateMovementAsync(Guid userId, Guid mapId, int x, int y, int gridX, int gridY, string direction, bool isMoving, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid userId, string status, CancellationToken cancellationToken = default);
    Task UpdateZoneAsync(Guid userId, Guid? zoneId, CancellationToken cancellationToken = default);
    Task UpdateHeartbeatAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RemovePresenceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserPresence>> GetStalePresencesAsync(int timeoutSeconds = 15, CancellationToken cancellationToken = default);
}
