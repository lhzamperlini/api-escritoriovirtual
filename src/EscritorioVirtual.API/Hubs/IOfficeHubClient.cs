namespace EscritorioVirtual.API.Hubs;

public interface IOfficeHubClient
{
    Task AvatarUpdated(Guid userId, object newAvatarConfig);
    Task UserJoined(object presence);
    Task UserLeft(Guid userId);
    Task UserMoved(Guid userId, int x, int y, int gridX, int gridY, string direction, bool isMoving);
    Task StatusChanged(Guid userId, string status);
    Task ReceiveProximityMessage(Guid senderId, string senderName, string text, int x, int y);
    Task ReceiveChannelMessage(Guid channelId, object message);
    Task RoomLockToggled(Guid zoneId, bool isLocked, Guid lockedByUserId);
    Task KnockRequested(Guid zoneId, Guid applicantUserId, string applicantName);
    Task KnockResponded(Guid zoneId, bool approved);
    Task WhiteboardUpdated(Guid zoneId, object patch);
}
