using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Workspaces.Dtos;

public class WorkspaceInviteDetailsDto
{
    public Guid WorkspaceId { get; set; }
    public string WorkspaceName { get; set; } = string.Empty;
    public string WorkspaceSlug { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; }
    public string InviterName { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}
