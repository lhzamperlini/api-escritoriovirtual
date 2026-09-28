using EscritorioVirtual.Application.Common.Interfaces.MultiTenancy;
using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Infrastructure.MultiTenancy;

public class WorkspaceContext : IWorkspaceContext
{
    public Guid? CurrentWorkspaceId { get; private set; }
    public WorkspaceRole? CurrentUserRole { get; private set; }
    public bool HasActiveWorkspace => CurrentWorkspaceId.HasValue;

    public void SetWorkspace(Guid workspaceId, WorkspaceRole? role)
    {
        CurrentWorkspaceId = workspaceId;
        CurrentUserRole = role;
    }
}
