using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceBySlug;

public record GetWorkspaceBySlugQuery(string Slug) : IRequest<WorkspaceDto>;

public class GetWorkspaceBySlugQueryHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspaceBySlugQuery, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(GetWorkspaceBySlugQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var workspace = await workspaceRepository.GetBySlugAsync(request.Slug.Trim().ToLowerInvariant(), cancellationToken);
        if (workspace is null)
        {
            throw new NotFoundException("Workspace", request.Slug);
        }

        var membership = await workspaceRepository.GetMembershipAsync(workspace.Id, userId.Value, cancellationToken);
        if (membership is null)
        {
            throw new ForbiddenException("Você não tem acesso a este Workspace.");
        }

        return new WorkspaceDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Slug = workspace.Slug,
            CreatedAt = workspace.CreatedAt,
            UserRole = membership.RoleId,
            MembersCount = workspace.Members.Count
        };
    }
}
