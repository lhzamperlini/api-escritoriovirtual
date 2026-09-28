using System.Text.RegularExpressions;
using FluentValidation;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Domain.Enums;
using MediatR;

namespace EscritorioVirtual.Application.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(string Name, string? Slug) : IRequest<WorkspaceDto>;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do Workspace é obrigatório.")
            .MaximumLength(255).WithMessage("O nome do Workspace não pode ultrapassar 255 caracteres.");

        RuleFor(x => x.Slug)
            .MaximumLength(100).WithMessage("O slug do Workspace não pode ultrapassar 100 caracteres.")
            .Matches("^[a-z0-9-]+$").When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("O slug deve conter apenas letras minúsculas, números e hífens.");
    }
}

public class CreateWorkspaceCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<CreateWorkspaceCommand, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? GenerateSlug(request.Name)
            : request.Slug.Trim().ToLowerInvariant();

        var slugExists = await workspaceRepository.ExistsSlugAsync(slug, cancellationToken: cancellationToken);
        if (slugExists)
        {
            // Se gerado automaticamente e já existe, adiciona sufixo aleatório
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";
            }
            else
            {
                throw new BadRequestException("Já existe um Workspace cadastrado com este slug.");
            }
        }

        var workspace = new Workspace(request.Name, slug, userId.Value);
        await workspaceRepository.AddWorkspaceAsync(workspace, cancellationToken);

        return new WorkspaceDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Slug = workspace.Slug,
            CreatedAt = workspace.CreatedAt,
            UserRole = WorkspaceRole.Owner,
            MembersCount = 1
        };
    }

    private static string GenerateSlug(string name)
    {
        var normalized = name.Normalize(System.Text.NormalizationForm.FormD);
        var chars = normalized
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray();
        var cleanString = new string(chars).ToLowerInvariant();
        cleanString = Regex.Replace(cleanString, @"[^a-z0-9\s-]", "");
        cleanString = Regex.Replace(cleanString, @"\s+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(cleanString) ? "workspace" : cleanString;
    }
}
