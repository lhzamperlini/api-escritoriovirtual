using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Workspaces.Dtos;

public class WorkspaceInviteDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string WorkspaceName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Email { get; set; }
    public WorkspaceRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
