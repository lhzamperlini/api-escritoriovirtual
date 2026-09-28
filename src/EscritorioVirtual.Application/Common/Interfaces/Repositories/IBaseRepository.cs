using System.Linq.Expressions;
using EscritorioVirtual.Application.Common.Filters;
using EscritorioVirtual.Application.Common.Interfaces.Dtos;

namespace EscritorioVirtual.Application.Common.Interfaces.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<TDto?> GetDtoAsync<TDto>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where TDto : IBaseDto<T, TDto>;
    Task<IEnumerable<TDto>> GetListDtoAsync<TDto>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where TDto : IBaseDto<T, TDto>;
    Task<T?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<PagedResult<TDto>> SearchAsync<TDto>(BaseFilterRequest<T, TDto> filterRequest, CancellationToken cancellationToken = default) where TDto : IBaseDto<T, TDto>;
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
}
