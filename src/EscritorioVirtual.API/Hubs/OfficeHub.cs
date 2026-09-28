using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.Aggregates.Presence;
using Microsoft.AspNetCore.SignalR;

namespace EscritorioVirtual.API.Hubs;

public class OfficeHub(
    IPresenceService presenceService,
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService) : Hub<IOfficeHubClient>
{
    public async Task JoinMap(Guid workspaceId, Guid mapId, int startX, int startY)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, mapGroup);

        var usuario = await usuarioRepository.GetAsync(u => u.Id == userId);
        var fullName = usuario?.FullName ?? "Colega";
        var avatarConfig = usuario?.AvatarConfig;

        var presence = new UserPresence
        {
            UserId = userId,
            FullName = fullName,
            AvatarConfig = avatarConfig,
            WorkspaceId = workspaceId,
            MapId = mapId,
            ConnectionId = Context.ConnectionId,
            X = startX,
            Y = startY,
            GridX = startX / 32,
            GridY = startY / 32,
            Status = "available",
            LastHeartbeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        await presenceService.AddOrUpdatePresenceAsync(presence);

        // Notifica colegas no mapa sobre a entrada
        await Clients.OthersInGroup(mapGroup).UserJoined(presence);

        // Envia todos os ocupantes já presentes para o recém-conectado
        var existingPresences = await presenceService.GetMapPresencesAsync(mapId);
        await Clients.Caller.CurrentMapPresences(existingPresences);
    }

    public async Task LeaveMap(Guid mapId)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, mapGroup);

        if (userId != Guid.Empty)
        {
            await presenceService.RemovePresenceAsync(userId);
            await Clients.OthersInGroup(mapGroup).UserLeft(userId);
        }
    }

    public async Task MoveStart(Guid mapId, string direction)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Clients.OthersInGroup(mapGroup).UserMoved(userId, 0, 0, 0, 0, direction, true);
    }

    public async Task MoveStop(Guid mapId, int x, int y, int gridX, int gridY)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";

        if (userId != Guid.Empty)
        {
            await presenceService.UpdateMovementAsync(userId, mapId, x, y, gridX, gridY, "idle", false);
        }

        await Clients.OthersInGroup(mapGroup).UserMoved(userId, x, y, gridX, gridY, "idle", false);
    }

    public async Task ChangeStatus(Guid mapId, string status)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";

        if (userId != Guid.Empty)
        {
            await presenceService.UpdateStatusAsync(userId, status);
        }

        await Clients.OthersInGroup(mapGroup).StatusChanged(userId, status);
    }

    public async Task Heartbeat(Guid mapId)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        if (userId != Guid.Empty)
        {
            await presenceService.UpdateHeartbeatAsync(userId);
        }
    }

    public async Task SendProximityMessage(Guid mapId, string text, int x, int y)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var usuario = await usuarioRepository.GetAsync(u => u.Id == userId);
        var senderName = usuario?.FullName ?? "Colega";

        // Proximity filtering: up to 10 tiles (320px)
        var presences = await presenceService.GetMapPresencesAsync(mapId);
        const double maxDistance = 10 * 32.0;

        var nearbyConnections = presences
            .Where(p => Math.Sqrt(Math.Pow(p.X - x, 2) + Math.Pow(p.Y - y, 2)) <= maxDistance && !string.IsNullOrEmpty(p.ConnectionId))
            .Select(p => p.ConnectionId)
            .ToList();

        if (nearbyConnections.Count > 0)
        {
            await Clients.Clients(nearbyConnections).ReceiveProximityMessage(userId, senderName, text, x, y);
        }
        else
        {
            await Clients.Group($"map_{mapId}").ReceiveProximityMessage(userId, senderName, text, x, y);
        }
    }

    public async Task JoinChannel(Guid channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"channel_{channelId}");
    }

    public async Task LeaveChannel(Guid channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel_{channelId}");
    }

    public async Task JoinZone(Guid zoneId)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        if (userId != Guid.Empty)
        {
            await presenceService.UpdateZoneAsync(userId, zoneId);
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, $"zone_{zoneId}");
    }

    public async Task LeaveZone(Guid zoneId)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        if (userId != Guid.Empty)
        {
            await presenceService.UpdateZoneAsync(userId, null);
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"zone_{zoneId}");
    }

    public async Task ToggleRoomLock(Guid mapId, Guid zoneId, bool isLocked)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        await Clients.Group($"map_{mapId}").RoomLockToggled(zoneId, isLocked, userId);
    }

    public async Task KnockRoom(Guid zoneId, string applicantName)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        await Clients.Group($"zone_{zoneId}").KnockRequested(zoneId, userId, applicantName);
    }

    public async Task RespondKnock(Guid zoneId, Guid targetUserId, bool approved)
    {
        await Clients.User(targetUserId.ToString()).KnockResponded(zoneId, approved);
        await Clients.Group($"zone_{zoneId}").KnockResponded(zoneId, approved);
    }

    public async Task BroadcastWhiteboardUpdate(Guid zoneId, object patch)
    {
        await Clients.OthersInGroup($"zone_{zoneId}").WhiteboardUpdated(zoneId, patch);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        if (userId != Guid.Empty)
        {
            // Marca como reconnecting permitindo o Grace Period de 15 segundos antes de remover
            await presenceService.UpdateStatusAsync(userId, "reconnecting");
            await Clients.All.StatusChanged(userId, "reconnecting");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
