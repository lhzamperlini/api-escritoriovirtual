using EscritorioVirtual.Application.Common.Interfaces.Repositories;
using EscritorioVirtual.Domain.AggregateRoot.Maps;

namespace EscritorioVirtual.Application.Maps.Interfaces;

public interface IMapRepository : IBaseRepository<Map>
{
    Task<Map?> GetMapWithDetailsAsync(Guid mapId, CancellationToken cancellationToken = default);
    Task<List<Map>> GetMapsByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task<Map> AddMapAsync(Map map, CancellationToken cancellationToken = default);
    Task UpdateMapAsync(Map map, CancellationToken cancellationToken = default);
    Task DeleteMapAsync(Guid mapId, CancellationToken cancellationToken = default);
}
