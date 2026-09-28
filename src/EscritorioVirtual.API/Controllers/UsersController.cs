using EscritorioVirtual.API.Hubs;
using EscritorioVirtual.Application.Avatars.Commands.UpdateAvatar;
using EscritorioVirtual.Application.Avatars.Dtos;
using EscritorioVirtual.Application.Avatars.Queries.GetMyAvatar;
using EscritorioVirtual.Application.Avatars.Queries.GetUserAvatar;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EscritorioVirtual.API.Controllers;

[Authorize]
[Route("api/users")]
public class UsersController(
    IHubContext<OfficeHub, IOfficeHubClient> officeHubContext,
    ICurrentUserService currentUserService) : ApiControllerBase
{
    [HttpGet("me/avatar")]
    [ProducesResponseType(typeof(AvatarConfigDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAvatar(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyAvatarQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:guid}/avatar")]
    [ProducesResponseType(typeof(AvatarConfigDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserAvatar(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUserAvatarQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("me/avatar")]
    [ProducesResponseType(typeof(AvatarConfigDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAvatar([FromBody] AvatarConfigDto request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateAvatarCommand(request), cancellationToken);

        var userId = currentUserService.UserId;
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            await officeHubContext.Clients.All.AvatarUpdated(userId.Value, result);
        }

        return Ok(result);
    }
}
