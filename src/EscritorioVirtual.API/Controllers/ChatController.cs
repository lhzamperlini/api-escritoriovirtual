using EscritorioVirtual.API.Hubs;
using EscritorioVirtual.Application.Chat.Commands;
using EscritorioVirtual.Application.Chat.DTOs;
using EscritorioVirtual.Application.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EscritorioVirtual.API.Controllers;

public class ChatController(
    ISender sender,
    IHubContext<OfficeHub, IOfficeHubClient> hubContext) : ApiControllerBase
{
    [HttpGet("workspaces/{workspaceId:guid}/channels")]
    public async Task<ActionResult<List<ChatChannelDto>>> GetChannels(Guid workspaceId)
    {
        var channels = await sender.Send(new GetChannelsQuery(workspaceId));
        return Ok(channels);
    }

    [HttpGet("channels/{channelId:guid}/messages")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChannelMessages(Guid channelId, [FromQuery] int take = 50)
    {
        var messages = await sender.Send(new GetChannelMessagesQuery(channelId, take));
        return Ok(messages);
    }

    [HttpPost("channels/{channelId:guid}/messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(Guid channelId, [FromBody] SendMessageRequest request)
    {
        var message = await sender.Send(new SendMessageCommand(channelId, request.Content));

        // Notifica membros do canal via SignalR
        await hubContext.Clients.Group($"channel_{channelId}").ReceiveChannelMessage(channelId, message);

        return Ok(message);
    }

    [HttpPost("workspaces/{workspaceId:guid}/direct")]
    public async Task<ActionResult<ChatChannelDto>> CreateDirectChannel(Guid workspaceId, [FromBody] CreateDirectRequest request)
    {
        var channel = await sender.Send(new CreateDirectChannelCommand(workspaceId, request.TargetUserId));
        return Ok(channel);
    }

    [HttpPost("workspaces/{workspaceId:guid}/zone")]
    public async Task<ActionResult<ChatChannelDto>> CreateZoneChannel(Guid workspaceId, [FromBody] CreateZoneRequest request)
    {
        var channel = await sender.Send(new CreateZoneChannelCommand(workspaceId, request.ZoneId, request.ZoneName));
        return Ok(channel);
    }

    [HttpPost("channels/{channelId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid channelId)
    {
        var success = await sender.Send(new MarkChannelAsReadCommand(channelId));
        return Ok(new { success });
    }
}

public record SendMessageRequest(string Content);
public record CreateDirectRequest(Guid TargetUserId);
public record CreateZoneRequest(Guid ZoneId, string ZoneName);
