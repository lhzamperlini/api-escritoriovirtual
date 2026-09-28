using EscritorioVirtual.Application.Workspaces.Dtos;
using EscritorioVirtual.Application.Workspaces.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Domain.Aggregates.Workspaces;
using EscritorioVirtual.Domain.Enums;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using EscritorioVirtual.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories.Workspaces;

public class WorkspaceRepository(AppDbContext appDbContext) : BaseRepository<Workspace>(appDbContext), IWorkspaceRepository
{
    private DbSet<Workspace> Workspaces => DbSet;
    private DbSet<WorkspaceUser> WorkspaceUsers => AppDbContext.Set<WorkspaceUser>();
    private DbSet<WorkspaceInvite> WorkspaceInvites => AppDbContext.Set<WorkspaceInvite>();

    public async Task<bool> ExistsSlugAsync(string slug, Guid? ignoreWorkspaceId = null, CancellationToken cancellationToken = default)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        return ignoreWorkspaceId.HasValue
            ? await Workspaces.AnyAsync(w => w.Slug == normalized && w.Id != ignoreWorkspaceId.Value, cancellationToken)
            : await Workspaces.AnyAsync(w => w.Slug == normalized, cancellationToken);
    }

    public async Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Workspaces
            .Include(w => w.Members)
                .ThenInclude(m => m.User)
            .Include(w => w.Invites)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Workspace?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        return await Workspaces
            .Include(w => w.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(w => w.Slug == normalized, cancellationToken);
    }

    public async Task<List<WorkspaceDto>> GetWorkspacesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await WorkspaceUsers
            .AsNoTracking()
            .Where(wu => wu.UserId == userId)
            .Select(wu => new WorkspaceDto
            {
                Id = wu.Workspace.Id,
                Name = wu.Workspace.Name,
                Slug = wu.Workspace.Slug,
                CreatedAt = wu.Workspace.CreatedAt,
                UserRole = wu.RoleId,
                MembersCount = wu.Workspace.Members.Count
            })
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkspaceUser?> GetMembershipAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == workspaceId && wu.UserId == userId, cancellationToken);
    }

    public async Task<WorkspaceInvite?> GetInviteByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await WorkspaceInvites
            .Include(i => i.Workspace)
            .Include(i => i.CreatedByUser)
            .FirstOrDefaultAsync(i => i.Code == code, cancellationToken);
    }

    public async Task<List<WorkspaceMemberDto>> GetMembersAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return await WorkspaceUsers
            .AsNoTracking()
            .Where(wu => wu.WorkspaceId == workspaceId)
            .Select(wu => new WorkspaceMemberDto
            {
                UserId = wu.UserId,
                FullName = wu.User.FullName,
                Email = wu.User.Email,
                Role = wu.RoleId,
                JoinedAt = wu.JoinedAt
            })
            .OrderBy(m => m.Role)
            .ThenBy(m => m.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
    {
        await Workspaces.AddAsync(workspace, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
    {
        Workspaces.Update(workspace);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default)
    {
        await WorkspaceUsers.AddAsync(membership, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default)
    {
        WorkspaceUsers.Update(membership);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveMembershipAsync(WorkspaceUser membership, CancellationToken cancellationToken = default)
    {
        WorkspaceUsers.Remove(membership);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInviteAsync(WorkspaceInvite invite, CancellationToken cancellationToken = default)
    {
        await WorkspaceInvites.AddAsync(invite, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInviteAsync(WorkspaceInvite invite, CancellationToken cancellationToken = default)
    {
        WorkspaceInvites.Update(invite);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }
}
