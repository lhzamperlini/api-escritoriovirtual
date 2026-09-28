using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Commands.UpdateMemberRole;

public record UpdateMemberRoleCommand(Guid WorkspaceId, Guid TargetUserId, WorkspaceRole NewRole) : IRequest<Unit>;

public class UpdateMemberRoleCommandValidator : AbstractValidator<UpdateMemberRoleCommand>
{
    public UpdateMemberRoleCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty();
        RuleFor(x => x.TargetUserId).NotEmpty();
        RuleFor(x => x.NewRole).IsInEnum();
    }
}

public class UpdateMemberRoleCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateMemberRoleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null || currentUserId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var currentMembership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, currentUserId.Value, cancellationToken);
        if (currentMembership is null || (currentMembership.RoleId != WorkspaceRole.Owner && currentMembership.RoleId != WorkspaceRole.Admin))
        {
            throw new ForbiddenException("Você não tem permissão para alterar permissões de membros neste Workspace.");
        }

        var targetMembership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, request.TargetUserId, cancellationToken);
        if (targetMembership is null)
        {
            throw new NotFoundException("Membro não encontrado neste Workspace.");
        }

        if (targetMembership.RoleId == WorkspaceRole.Owner && currentMembership.RoleId != WorkspaceRole.Owner)
        {
            throw new ForbiddenException("Apenas o Proprietário pode alterar as permissões de outro Proprietário.");
        }

        targetMembership.UpdateRole(request.NewRole);
        await workspaceRepository.UpdateMembershipAsync(targetMembership, cancellationToken);

        return Unit.Value;
    }
}
