using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Queries.GetInviteDetails;

public record GetInviteDetailsQuery(string Code) : IRequest<WorkspaceInviteDetailsDto>;

public class GetInviteDetailsQueryHandler(
    IWorkspaceRepository workspaceRepository) : IRequestHandler<GetInviteDetailsQuery, WorkspaceInviteDetailsDto>
{
    public async Task<WorkspaceInviteDetailsDto> Handle(GetInviteDetailsQuery request, CancellationToken cancellationToken)
    {
        var invite = await workspaceRepository.GetInviteByCodeAsync(request.Code.Trim(), cancellationToken);
        if (invite is null)
        {
            return new WorkspaceInviteDetailsDto
            {
                IsValid = false,
                ErrorMessage = "Convite não encontrado."
            };
        }

        if (!invite.IsValid())
        {
            return new WorkspaceInviteDetailsDto
            {
                WorkspaceId = invite.WorkspaceId,
                WorkspaceName = invite.Workspace?.Name ?? "Workspace",
                WorkspaceSlug = invite.Workspace?.Slug ?? "",
                Role = invite.RoleId,
                IsValid = false,
                ErrorMessage = "Este convite expirou ou foi desativado."
            };
        }

        return new WorkspaceInviteDetailsDto
        {
            WorkspaceId = invite.WorkspaceId,
            WorkspaceName = invite.Workspace?.Name ?? "Workspace",
            WorkspaceSlug = invite.Workspace?.Slug ?? "",
            Role = invite.RoleId,
            InviterName = invite.CreatedByUser?.FullName ?? "Administrador",
            IsValid = true
        };
    }
}
