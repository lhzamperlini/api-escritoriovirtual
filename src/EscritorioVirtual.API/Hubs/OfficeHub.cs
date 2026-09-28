using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace EscritorioVirtual.API.Hubs;

public class OfficeHub(ICurrentUserService currentUserService) : Hub<IOfficeHubClient>
{
    public async Task JoinMap(Guid workspaceId, Guid mapId, int startX, int startY)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, mapGroup);

        await Clients.OthersInGroup(mapGroup).UserJoined(new
        {
            userId,
            connectionId = Context.ConnectionId,
            mapId,
            x = startX,
            y = startY,
            gridX = startX / 32,
            gridY = startY / 32,
            status = "available",
            lastHeartbeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task LeaveMap(Guid mapId)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, mapGroup);
        await Clients.OthersInGroup(mapGroup).UserLeft(userId);
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
        await Clients.OthersInGroup(mapGroup).UserMoved(userId, x, y, gridX, gridY, "idle", false);
    }

    public async Task ChangeStatus(Guid mapId, string status)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Clients.OthersInGroup(mapGroup).StatusChanged(userId, status);
    }

    public Task Heartbeat()
    {
        return Task.CompletedTask;
    }

    public async Task SendProximityMessage(Guid mapId, string text, int x, int y)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var mapGroup = $"map_{mapId}";
        await Clients.Group(mapGroup).ReceiveProximityMessage(userId, "Colega", text, x, y);
    }

    public async Task JoinZone(Guid zoneId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"zone_{zoneId}");
    }

    public async Task LeaveZone(Guid zoneId)
    {
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
            await Clients.All.UserLeft(userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
