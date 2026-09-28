using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.Aggregates.Presence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscritorioVirtual.API.Controllers;

[Authorize]
public class PresenceController(IPresenceService presenceService) : ApiControllerBase
{
    [HttpGet("api/maps/{mapId:guid}/presence")]
    [ProducesResponseType(typeof(List<UserPresence>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMapPresence(Guid mapId, CancellationToken cancellationToken)
    {
        var presences = await presenceService.GetMapPresencesAsync(mapId, cancellationToken);
        return Ok(presences);
    }
}
