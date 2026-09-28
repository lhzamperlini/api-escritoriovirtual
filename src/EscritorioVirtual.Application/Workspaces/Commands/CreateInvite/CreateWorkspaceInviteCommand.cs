using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Commands.CreateInvite;

public record CreateWorkspaceInviteCommand(
    Guid WorkspaceId,
    WorkspaceRole Role = WorkspaceRole.Member,
    string? Email = null,
    int? ExpirationDays = 7
) : IRequest<WorkspaceInviteDto>;

public class CreateWorkspaceInviteCommandValidator : AbstractValidator<CreateWorkspaceInviteCommand>
{
    public CreateWorkspaceInviteCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId é obrigatório.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Role inválida.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("O e-mail informado não é válido.");
    }
}

public class CreateWorkspaceInviteCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<CreateWorkspaceInviteCommand, WorkspaceInviteDto>
{
    public async Task<WorkspaceInviteDto> Handle(CreateWorkspaceInviteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace is null)
        {
            throw new NotFoundException("Workspace", request.WorkspaceId);
        }

        var membership = await workspaceRepository.GetMembershipAsync(request.WorkspaceId, userId.Value, cancellationToken);
        if (membership is null || (membership.RoleId != WorkspaceRole.Owner && membership.RoleId != WorkspaceRole.Admin))
        {
            throw new ForbiddenException("Apenas Administradores ou Proprietários do Workspace podem gerar convites.");
        }

        DateTime? expiresAt = request.ExpirationDays.HasValue && request.ExpirationDays.Value > 0
            ? DateTime.UtcNow.AddDays(request.ExpirationDays.Value)
            : null;

        var invite = workspace.CreateInvite(userId.Value, request.Role, request.Email, expiresAt);
        await workspaceRepository.AddInviteAsync(invite, cancellationToken);

        return new WorkspaceInviteDto
        {
            Id = invite.Id,
            WorkspaceId = workspace.Id,
            WorkspaceName = workspace.Name,
            Code = invite.Code,
            Email = invite.Email,
            Role = invite.RoleId,
            CreatedAt = invite.CreatedAt,
            ExpiresAt = invite.ExpiresAt,
            IsActive = invite.IsActive
        };
    }
}
