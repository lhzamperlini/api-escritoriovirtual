using EscritorioVirtual.Application.Whiteboards.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories.Whiteboards;

public class WhiteboardRepository(AppDbContext appDbContext)
    : BaseRepository<Whiteboard>(appDbContext), IWhiteboardRepository
{
    public async Task<Whiteboard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Whiteboard?> GetByZoneIdAsync(Guid workspaceId, Guid zoneId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId && w.ZoneId == zoneId, cancellationToken);
    }

    public async Task<List<Whiteboard>> GetWorkspaceWhiteboardsAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(w => w.WorkspaceId == workspaceId)
            .OrderByDescending(w => w.LastUpdated)
            .ToListAsync(cancellationToken);
    }

    public async Task<Whiteboard> CreateAsync(Whiteboard whiteboard, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(whiteboard, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
        return whiteboard;
    }

    public async Task UpdateAsync(Whiteboard whiteboard, CancellationToken cancellationToken = default)
    {
        DbSet.Update(whiteboard);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }
}
