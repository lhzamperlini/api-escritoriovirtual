using System.Collections.Concurrent;
using EscritorioVirtual.Application.Common.Interfaces.Services;

namespace EscritorioVirtual.Infrastructure.Services;

public class RoomAccessService : IRoomAccessService
{
    private class RoomState
    {
        public bool IsLocked { get; set; }
        public Guid? LockedByUserId { get; set; }
        public HashSet<Guid> AllowedGuests { get; } = new();
    }

    private static readonly ConcurrentDictionary<Guid, RoomState> Rooms = new();

    public bool IsRoomLocked(Guid zoneId)
    {
        return Rooms.TryGetValue(zoneId, out var state) && state.IsLocked;
    }

    public Guid? GetLockedBy(Guid zoneId)
    {
        return Rooms.TryGetValue(zoneId, out var state) ? state.LockedByUserId : null;
    }

    public void SetRoomLock(Guid zoneId, bool isLocked, Guid lockedByUserId)
    {
        var state = Rooms.GetOrAdd(zoneId, _ => new RoomState());
        state.IsLocked = isLocked;
        state.LockedByUserId = isLocked ? lockedByUserId : null;
        if (!isLocked)
        {
            state.AllowedGuests.Clear();
        }
    }

    public void ApproveGuest(Guid zoneId, Guid guestUserId)
    {
        var state = Rooms.GetOrAdd(zoneId, _ => new RoomState());
        state.AllowedGuests.Add(guestUserId);
    }

    public bool CanEnterRoom(Guid workspaceId, Guid zoneId, Guid userId, bool isOwnerOrAdmin)
    {
        // Regra US04: Administrador ou Owner possui Chave-Mestra irrestrita
        if (isOwnerOrAdmin)
        {
            return true;
        }

        if (!Rooms.TryGetValue(zoneId, out var state) || !state.IsLocked)
        {
            return true; // Sala destrancada
        }

        // Anfitrião que trancou tem acesso direto
        if (state.LockedByUserId == userId)
        {
            return true;
        }

        // Convidado aprovado via Knock
        return state.AllowedGuests.Contains(userId);
    }
}
