using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Queries.GetUserWorkspaces;

public record GetUserWorkspacesQuery : IRequest<List<WorkspaceDto>>;

public class GetUserWorkspacesQueryHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserWorkspacesQuery, List<WorkspaceDto>>
{
    public async Task<List<WorkspaceDto>> Handle(GetUserWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        return await workspaceRepository.GetWorkspacesByUserIdAsync(userId.Value, cancellationToken);
    }
}
