using EscritorioVirtual.Application.Whiteboards.Commands.CreateWhiteboard;
using EscritorioVirtual.Application.Whiteboards.Commands.SaveSnapshot;
using EscritorioVirtual.Application.Whiteboards.DTOs;
using EscritorioVirtual.Application.Whiteboards.Queries.GetWhiteboard;
using EscritorioVirtual.Application.Whiteboards.Queries.GetWorkspaceWhiteboards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EscritorioVirtual.API.Controllers;

[Route("api/[controller]")]
public class WhiteboardsController(ISender sender) : ApiControllerBase
{
    [HttpGet("/api/workspaces/{workspaceId:guid}/whiteboards")]
    public async Task<ActionResult<object>> GetWorkspaceWhiteboards(Guid workspaceId, [FromQuery] Guid? zoneId = null)
    {
        if (zoneId.HasValue && zoneId.Value != Guid.Empty)
        {
            var zoneBoard = await sender.Send(new GetWhiteboardQuery(null, workspaceId, zoneId.Value));
            return Ok(zoneBoard);
        }

        var boards = await sender.Send(new GetWorkspaceWhiteboardsQuery(workspaceId));
        return Ok(boards);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WhiteboardDto>> GetById(Guid id)
    {
        var board = await sender.Send(new GetWhiteboardQuery(id, null, null));
        if (board is null) return NotFound();
        return Ok(board);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WhiteboardDto>> SaveSnapshot(Guid id, [FromBody] SaveSnapshotRequest request)
    {
        var updated = await sender.Send(new SaveWhiteboardSnapshotCommand(id, request.DocumentData));
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpPost("/api/workspaces/{workspaceId:guid}/whiteboards")]
    public async Task<ActionResult<WhiteboardDto>> Create(Guid workspaceId, [FromBody] CreateWhiteboardRequest request)
    {
        var created = await sender.Send(new CreateWhiteboardCommand(workspaceId, request.Name, request.ZoneId, request.InitialData));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

public record SaveSnapshotRequest(string DocumentData);
public record CreateWhiteboardRequest(string Name, Guid? ZoneId = null, string? InitialData = null);
