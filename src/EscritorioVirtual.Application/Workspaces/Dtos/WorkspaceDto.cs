using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Workspaces.Dtos;

public class WorkspaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public WorkspaceRole? UserRole { get; set; }
    public int MembersCount { get; set; }
}
