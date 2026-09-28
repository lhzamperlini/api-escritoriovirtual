using EscritorioVirtual.Application.Maps.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Maps;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories.Maps;

public class MapRepository(AppDbContext appDbContext)
    : BaseRepository<Map>(appDbContext), IMapRepository
{
    public async Task<Map?> GetMapWithDetailsAsync(Guid mapId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(m => m.Objects)
            .Include(m => m.Zones)
            .FirstOrDefaultAsync(m => m.Id == mapId, cancellationToken);
    }

    public async Task<List<Map>> GetMapsByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(m => m.WorkspaceId == workspaceId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Map> AddMapAsync(Map map, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(map, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
        return map;
    }

    public async Task UpdateMapAsync(Map map, CancellationToken cancellationToken = default)
    {
        DbSet.Update(map);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMapAsync(Guid mapId, CancellationToken cancellationToken = default)
    {
        var map = await DbSet.FindAsync([mapId], cancellationToken);
        if (map != null)
        {
            DbSet.Remove(map);
            await AppDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
