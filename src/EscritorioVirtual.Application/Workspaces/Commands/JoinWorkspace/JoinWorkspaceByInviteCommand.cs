using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.Aggregates.Workspaces;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Commands.JoinWorkspace;

public record JoinWorkspaceByInviteCommand(string Code) : IRequest<WorkspaceDto>;

public class JoinWorkspaceByInviteCommandValidator : AbstractValidator<JoinWorkspaceByInviteCommand>
{
    public JoinWorkspaceByInviteCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("O código de convite é obrigatório.");
    }
}

public class JoinWorkspaceByInviteCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<JoinWorkspaceByInviteCommand, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(JoinWorkspaceByInviteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var invite = await workspaceRepository.GetInviteByCodeAsync(request.Code.Trim(), cancellationToken);
        if (invite is null || !invite.IsValid())
        {
            throw new BadRequestException("Este convite é inválido ou já expirou.");
        }

        if (!string.IsNullOrWhiteSpace(invite.Email))
        {
            var userEmail = currentUserService.Email;
            if (!string.Equals(userEmail, invite.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException($"Este convite foi emitido exclusivamente para o e-mail {invite.Email}.");
            }
        }

        var existingMembership = await workspaceRepository.GetMembershipAsync(invite.WorkspaceId, userId.Value, cancellationToken);
        if (existingMembership is not null)
        {
            var existingWorkspace = await workspaceRepository.GetByIdWithMembersAsync(invite.WorkspaceId, cancellationToken);
            return new WorkspaceDto
            {
                Id = existingWorkspace!.Id,
                Name = existingWorkspace.Name,
                Slug = existingWorkspace.Slug,
                CreatedAt = existingWorkspace.CreatedAt,
                UserRole = existingMembership.RoleId,
                MembersCount = existingWorkspace.Members.Count
            };
        }

        var membership = new WorkspaceUser(invite.WorkspaceId, userId.Value, invite.RoleId);
        await workspaceRepository.AddMembershipAsync(membership, cancellationToken);

        var workspace = await workspaceRepository.GetByIdWithMembersAsync(invite.WorkspaceId, cancellationToken);

        return new WorkspaceDto
        {
            Id = workspace!.Id,
            Name = workspace.Name,
            Slug = workspace.Slug,
            CreatedAt = workspace.CreatedAt,
            UserRole = invite.RoleId,
            MembersCount = workspace.Members.Count
        };
    }
}
