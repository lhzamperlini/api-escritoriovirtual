using EscritorioVirtual.Application.Workspaces.Commands.CreateInvite;
using EscritorioVirtual.Application.Workspaces.Commands.CreateWorkspace;
using EscritorioVirtual.Application.Workspaces.Commands.JoinWorkspace;
using EscritorioVirtual.Application.Workspaces.Commands.RemoveMember;
using EscritorioVirtual.Application.Workspaces.Commands.UpdateMemberRole;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Queries.GetInviteDetails;
using EscritorioVirtual.Application.Workspaces.Queries.GetUserWorkspaces;
using EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceById;
using EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceBySlug;
using EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceMembers;
using EscritorioVirtual.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscritorioVirtual.API.Controllers;

[Route("api/[controller]")]
public class WorkspacesController : ApiControllerBase
{
    /// <summary>
    /// US01 - Criação do Workspace
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkspaceDto>> Create([FromBody] CreateWorkspaceCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// US03 - Troca e Seletor de Workspaces do usuário logado
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<WorkspaceDto>>> GetUserWorkspaces()
    {
        var result = await Mediator.Send(new GetUserWorkspacesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Obter detalhes de um Workspace por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkspaceDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetWorkspaceByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Obter detalhes de um Workspace por Slug (URL amigável)
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<WorkspaceDto>> GetBySlug(string slug)
    {
        var result = await Mediator.Send(new GetWorkspaceBySlugQuery(slug));
        return Ok(result);
    }

    /// <summary>
    /// Obter membros de um Workspace
    /// </summary>
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<WorkspaceMemberDto>>> GetMembers(Guid id)
    {
        var result = await Mediator.Send(new GetWorkspaceMembersQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// US02 - Gerar convite (link / email) para novos funcionários
    /// </summary>
    [HttpPost("{id:guid}/invites")]
    public async Task<ActionResult<WorkspaceInviteDto>> CreateInvite(Guid id, [FromBody] CreateInviteRequest request)
    {
        var command = new CreateWorkspaceInviteCommand(
            WorkspaceId: id,
            Role: request.Role,
            Email: request.Email,
            ExpirationDays: request.ExpirationDays ?? 7
        );

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// US02 - Obter informações prévias de um convite a partir do código/link
    /// </summary>
    [HttpGet("invites/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<WorkspaceInviteDetailsDto>> GetInviteDetails(string code)
    {
        var result = await Mediator.Send(new GetInviteDetailsQuery(code));
        return Ok(result);
    }

    /// <summary>
    /// US02 - Aceitar convite e ingressar no Workspace
    /// </summary>
    [HttpPost("invites/{code}/join")]
    public async Task<ActionResult<WorkspaceDto>> JoinByInvite(string code)
    {
        var result = await Mediator.Send(new JoinWorkspaceByInviteCommand(code));
        return Ok(result);
    }

    /// <summary>
    /// Alterar permissão/papel de um membro
    /// </summary>
    [HttpPut("{id:guid}/members/{userId:guid}/role")]
    public async Task<ActionResult> UpdateRole(Guid id, Guid userId, [FromBody] UpdateRoleRequest request)
    {
        await Mediator.Send(new UpdateMemberRoleCommand(id, userId, request.Role));
        return NoContent();
    }

    /// <summary>
    /// Remover membro ou sair do Workspace
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<ActionResult> RemoveMember(Guid id, Guid userId)
    {
        await Mediator.Send(new RemoveMemberCommand(id, userId));
        return NoContent();
    }
}

public record CreateInviteRequest(WorkspaceRole Role = WorkspaceRole.Member, string? Email = null, int? ExpirationDays = 7);
public record UpdateRoleRequest(WorkspaceRole Role);
