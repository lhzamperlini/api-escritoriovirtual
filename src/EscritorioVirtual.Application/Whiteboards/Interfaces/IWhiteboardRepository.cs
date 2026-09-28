using EscritorioVirtual.Domain.AggregateRoot.Whiteboards;

namespace EscritorioVirtual.Application.Whiteboards.Interfaces;

public interface IWhiteboardRepository
{
    Task<Whiteboard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Whiteboard?> GetByZoneIdAsync(Guid workspaceId, Guid zoneId, CancellationToken cancellationToken = default);
    Task<List<Whiteboard>> GetWorkspaceWhiteboardsAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task<Whiteboard> CreateAsync(Whiteboard whiteboard, CancellationToken cancellationToken = default);
    Task UpdateAsync(Whiteboard whiteboard, CancellationToken cancellationToken = default);
}
