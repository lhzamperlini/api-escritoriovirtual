using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceById;

public record GetWorkspaceByIdQuery(Guid Id) : IRequest<WorkspaceDto>;

public class GetWorkspaceByIdQueryHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.Id, cancellationToken);
        if (workspace is null)
        {
            throw new NotFoundException("Workspace", request.Id);
        }

        var membership = await workspaceRepository.GetMembershipAsync(request.Id, userId.Value, cancellationToken);
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
