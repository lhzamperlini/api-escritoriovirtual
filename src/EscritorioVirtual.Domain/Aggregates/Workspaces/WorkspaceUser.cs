using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Domain.Aggregates.Workspaces;

public class WorkspaceUser
{
    public Guid WorkspaceId { get; private set; }
    public Workspace Workspace { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public Usuario User { get; private set; } = null!;

    public WorkspaceRole RoleId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    protected WorkspaceUser() { }

    public WorkspaceUser(Guid workspaceId, Guid userId, WorkspaceRole role)
    {
        WorkspaceId = workspaceId;
        UserId = userId;
        RoleId = role;
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateRole(WorkspaceRole newRole)
    {
        RoleId = newRole;
    }
}
