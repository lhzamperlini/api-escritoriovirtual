using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Application.Common.Interfaces.MultiTenancy;

public interface IWorkspaceContext
{
    Guid? CurrentWorkspaceId { get; }
    WorkspaceRole? CurrentUserRole { get; }
    bool HasActiveWorkspace { get; }
    void SetWorkspace(Guid workspaceId, WorkspaceRole? role);
}
