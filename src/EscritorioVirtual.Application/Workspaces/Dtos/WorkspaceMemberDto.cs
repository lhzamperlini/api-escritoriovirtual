using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Workspaces.Dtos;

public class WorkspaceMemberDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
