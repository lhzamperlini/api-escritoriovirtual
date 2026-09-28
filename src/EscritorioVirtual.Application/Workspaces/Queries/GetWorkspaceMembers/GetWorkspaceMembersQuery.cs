using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Queries.GetWorkspaceMembers;

public record GetWorkspaceMembersQuery(Guid WorkspaceId) : IRequest<List<WorkspaceMemberDto>>;

public class GetWorkspaceMembersQueryHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspaceMembersQuery, List<WorkspaceMemberDto>>
{
    public async Task<List<WorkspaceMemberDto>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var membership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, userId.Value, cancellationToken);
        if (membership is null)
        {
            throw new ForbiddenException("Você não tem acesso aos membros deste Workspace.");
        }

        return await workspaceRepository.GetMembersAsync(request.WorkspaceId, cancellationToken);
    }
}
