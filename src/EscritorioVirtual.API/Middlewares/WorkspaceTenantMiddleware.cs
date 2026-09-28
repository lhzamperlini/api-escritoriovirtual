using System.Security.Claims;
using EscritorioVirtual.API.Attributes;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.MultiTenancy;
using EscritorioVirtual.Application.Workspaces.Interfaces;

namespace EscritorioVirtual.API.Middlewares;

public class WorkspaceTenantMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Workspace-Id";

    public async Task InvokeAsync(
        HttpContext context,
        IWorkspaceContext workspaceContext,
        IWorkspaceRepository workspaceRepository)
    {
        var endpoint = context.GetEndpoint();
        var requiresWorkspace = endpoint?.Metadata.GetMetadata<RequireWorkspaceAttribute>() != null;

        var headerValue = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? context.Request.Query["workspaceId"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(headerValue) && Guid.TryParse(headerValue, out var workspaceId))
        {
            var subClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(subClaim, out var userId))
            {
                var membership = await workspaceRepository.GetMembershipAsync(workspaceId, userId, context.RequestAborted);
                if (membership != null)
                {
                    workspaceContext.SetWorkspace(workspaceId, membership.RoleId);
                }
                else if (requiresWorkspace)
                {
                    throw new ForbiddenException("Você não possui acesso a este Workspace.");
                }
            }
            else if (requiresWorkspace)
            {
                throw new UnauthorizedException("Usuário não autenticado para acessar este Workspace.");
            }
        }
        else if (requiresWorkspace)
        {
            throw new BadRequestException($"O cabeçalho '{HeaderName}' é obrigatório para acessar este recurso.");
        }

        await next(context);
    }
}
