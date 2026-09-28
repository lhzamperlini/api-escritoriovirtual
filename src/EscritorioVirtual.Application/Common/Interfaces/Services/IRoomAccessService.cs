namespace EscritorioVirtual.Application.Common.Interfaces.Services;

public interface IRoomAccessService
{
    bool IsRoomLocked(Guid zoneId);
    Guid? GetLockedBy(Guid zoneId);
    void SetRoomLock(Guid zoneId, bool isLocked, Guid lockedByUserId);
    void ApproveGuest(Guid zoneId, Guid guestUserId);
    bool CanEnterRoom(Guid workspaceId, Guid zoneId, Guid userId, bool isOwnerOrAdmin);
}
