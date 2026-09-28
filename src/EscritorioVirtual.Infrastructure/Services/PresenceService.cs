using System.Collections.Concurrent;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.Aggregates.Presence;

namespace EscritorioVirtual.Infrastructure.Services;

public class PresenceService : IPresenceService
{
    private static readonly ConcurrentDictionary<Guid, UserPresence> Presences = new();

    public Task AddOrUpdatePresenceAsync(UserPresence presence, CancellationToken cancellationToken = default)
    {
        Presences.AddOrUpdate(presence.UserId, presence, (_, _) => presence);
        return Task.CompletedTask;
    }

    public Task<UserPresence?> GetUserPresenceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Presences.TryGetValue(userId, out var presence);
        return Task.FromResult(presence);
    }

    public Task<List<UserPresence>> GetMapPresencesAsync(Guid mapId, CancellationToken cancellationToken = default)
    {
        var list = Presences.Values.Where(p => p.MapId == mapId).ToList();
        return Task.FromResult(list);
    }

    public Task UpdateMovementAsync(Guid userId, Guid mapId, int x, int y, int gridX, int gridY, string direction, bool isMoving, CancellationToken cancellationToken = default)
    {
        if (Presences.TryGetValue(userId, out var presence))
        {
            presence.X = x;
            presence.Y = y;
            presence.GridX = gridX;
            presence.GridY = gridY;
            presence.Direction = direction;
            presence.IsMoving = isMoving;
            presence.TouchHeartbeat();
        }
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid userId, string status, CancellationToken cancellationToken = default)
    {
        if (Presences.TryGetValue(userId, out var presence))
        {
            presence.Status = status;
            presence.TouchHeartbeat();
        }
        return Task.CompletedTask;
    }

    public Task UpdateZoneAsync(Guid userId, Guid? zoneId, CancellationToken cancellationToken = default)
    {
        if (Presences.TryGetValue(userId, out var presence))
        {
            presence.CurrentZoneId = zoneId;
            presence.TouchHeartbeat();
        }
        return Task.CompletedTask;
    }

    public Task UpdateHeartbeatAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (Presences.TryGetValue(userId, out var presence))
        {
            presence.TouchHeartbeat();
        }
        return Task.CompletedTask;
    }

    public Task RemovePresenceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Presences.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    public Task<List<UserPresence>> GetStalePresencesAsync(int timeoutSeconds = 15, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var stale = Presences.Values
            .Where(p => p.IsGracePeriodExpired(now, timeoutSeconds))
            .ToList();
        return Task.FromResult(stale);
    }
}
