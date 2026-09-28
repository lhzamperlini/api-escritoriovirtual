using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Commands.RemoveMember;

public record RemoveMemberCommand(Guid WorkspaceId, Guid TargetUserId) : IRequest<Unit>;

public class RemoveMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<RemoveMemberCommand, Unit>
{
    public async Task<Unit> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null || currentUserId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var isSelfRemoval = currentUserId.Value == request.TargetUserId;

        var currentMembership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, currentUserId.Value, cancellationToken);
        if (currentMembership is null)
        {
            throw new ForbiddenException("Você não tem acesso a este Workspace.");
        }

        if (!isSelfRemoval && currentMembership.RoleId != WorkspaceRole.Owner && currentMembership.RoleId != WorkspaceRole.Admin)
        {
            throw new ForbiddenException("Você não tem permissão para remover membros deste Workspace.");
        }

        var targetMembership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, request.TargetUserId, cancellationToken);
        if (targetMembership is null)
        {
            throw new NotFoundException("Membro não encontrado neste Workspace.");
        }

        if (targetMembership.RoleId == WorkspaceRole.Owner)
        {
            throw new BadRequestException("O proprietário do Workspace não pode ser removido.");
        }

        await workspaceRepository.RemoveMembershipAsync(targetMembership, cancellationToken);

        return Unit.Value;
    }
}
