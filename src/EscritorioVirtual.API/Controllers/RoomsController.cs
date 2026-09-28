using EscritorioVirtual.API.Hubs;
using EscritorioVirtual.Application.Common.Interfaces.MultiTenancy;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EscritorioVirtual.API.Controllers;

[Route("api/[controller]")]
public class RoomsController(
    IRoomAccessService roomAccessService,
    ICurrentUserService currentUserService,
    IHubContext<OfficeHub, IOfficeHubClient> hubContext) : ApiControllerBase
{
    [HttpGet("{zoneId:guid}/status")]
    public ActionResult<RoomStatusResponse> GetStatus(Guid zoneId)
    {
        var isLocked = roomAccessService.IsRoomLocked(zoneId);
        var lockedBy = roomAccessService.GetLockedBy(zoneId);
        return Ok(new RoomStatusResponse(zoneId, isLocked, lockedBy));
    }

    [HttpPost("{zoneId:guid}/lock")]
    public async Task<ActionResult<RoomStatusResponse>> ToggleLock(Guid zoneId, [FromBody] ToggleLockRequest request)
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        roomAccessService.SetRoomLock(zoneId, request.IsLocked, userId);

        await hubContext.Clients.Group($"zone_{zoneId}").RoomLockToggled(zoneId, request.IsLocked, userId);

        return Ok(new RoomStatusResponse(zoneId, request.IsLocked, request.IsLocked ? userId : null));
    }

    [HttpPost("{zoneId:guid}/knock")]
    public async Task<IActionResult> Knock(Guid zoneId, [FromBody] KnockRequest request)
    {
        var applicantId = currentUserService.UserId ?? Guid.Empty;
        await hubContext.Clients.Group($"zone_{zoneId}").KnockRequested(zoneId, applicantId, request.ApplicantName);
        return Ok(new { success = true });
    }

    [HttpPost("{zoneId:guid}/respond-knock")]
    public async Task<IActionResult> RespondKnock(Guid zoneId, [FromBody] RespondKnockRequest request)
    {
        if (request.Approved)
        {
            roomAccessService.ApproveGuest(zoneId, request.TargetUserId);
        }

        await hubContext.Clients.User(request.TargetUserId.ToString()).KnockResponded(zoneId, request.Approved);
        await hubContext.Clients.Group($"zone_{zoneId}").KnockResponded(zoneId, request.Approved);

        return Ok(new { success = true, approved = request.Approved });
    }
}

public record RoomStatusResponse(Guid ZoneId, bool IsLocked, Guid? LockedByUserId);
public record ToggleLockRequest(bool IsLocked);
public record KnockRequest(string ApplicantName);
public record RespondKnockRequest(Guid TargetUserId, bool Approved);
