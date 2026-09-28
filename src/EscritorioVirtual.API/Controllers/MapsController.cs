using EscritorioVirtual.Application.Maps.Commands.CreateMap;
using EscritorioVirtual.Application.Maps.Commands.ImportTiledMap;
using EscritorioVirtual.Application.Maps.Commands.UpdateMapLayout;
using EscritorioVirtual.Application.Maps.Dtos;
using EscritorioVirtual.Application.Maps.Queries.GetMapById;
using EscritorioVirtual.Application.Maps.Queries.GetWorkspaceMaps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscritorioVirtual.API.Controllers;

[Authorize]
public class MapsController : ApiControllerBase
{
    [HttpGet("api/workspaces/{workspaceId:guid}/maps")]
    [ProducesResponseType(typeof(List<MapDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkspaceMaps(Guid workspaceId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetWorkspaceMapsQuery(workspaceId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("api/workspaces/{workspaceId:guid}/maps")]
    [ProducesResponseType(typeof(MapDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMap(Guid workspaceId, [FromBody] CreateMapCommand command, CancellationToken cancellationToken)
    {
        var finalCommand = command with { WorkspaceId = workspaceId };
        var result = await Mediator.Send(finalCommand, cancellationToken);
        return CreatedAtAction(nameof(GetMapById), new { mapId = result.Id }, result);
    }

    [HttpGet("api/maps/{mapId:guid}")]
    [ProducesResponseType(typeof(MapDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMapById(Guid mapId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMapByIdQuery(mapId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("api/maps/{mapId:guid}/layout")]
    [ProducesResponseType(typeof(MapDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLayout(Guid mapId, [FromBody] UpdateMapLayoutDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateMapLayoutCommand(mapId, dto.Objects, dto.Zones);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("api/maps/{mapId:guid}/import-tiled")]
    [ProducesResponseType(typeof(MapDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportTiled(Guid mapId, [FromBody] string tiledJson, CancellationToken cancellationToken)
    {
        var command = new ImportTiledMapCommand(mapId, tiledJson);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
