using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Domain.Aggregates.Workspaces;

namespace EscritorioVirtual.Application.Workspaces.Interfaces;

public interface IWorkspaceRepository
{
    Task<bool> ExistsSlugAsync(string slug, Guid? ignoreWorkspaceId = null, CancellationToken cancellationToken = default);
    Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Workspace?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<List<WorkspaceDto>> GetWorkspacesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkspaceUser?> GetMembershipAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default);
    Task<WorkspaceInvite?> GetInviteByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<WorkspaceMemberDto>> GetMembersAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task AddWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task UpdateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task AddMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default);
    Task UpdateMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default);
    Task RemoveMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default);
    Task AddInviteAsync(WorkspaceInvite invite, CancellationToken cancellationToken = default);
    Task UpdateInviteAsync(WorkspaceInvite invite, CancellationToken cancellationToken = default);
}
