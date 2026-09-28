using System.Linq.Expressions;
using EscritorioVirtual.Application.Common.Filters;
using EscritorioVirtual.Application.Common.Interfaces.Dtos;
using EscritorioVirtual.Application.Common.Interfaces.Repositories;
using EscritorioVirtual.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories;
public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
	protected AppDbContext AppDbContext { get; }
	protected DbSet<T> DbSet { get; }

	public BaseRepository(AppDbContext appDbContext)
	{
		AppDbContext = appDbContext;
		DbSet = AppDbContext.Set<T>();
	}

	public async Task<PagedResult<TDto>> SearchAsync<TDto>(BaseFilterRequest<T, TDto> filterRequest, CancellationToken cancellationToken) where TDto : IBaseDto<T, TDto>
	{
		var expression = filterRequest.Expression();

		var baseQuery = DbSet.AsNoTracking().Where(expression);

		var totalCount = await baseQuery.CountAsync(cancellationToken);

		if (totalCount == 0)
			return PagedResult<TDto>.Empty;

		var query = await baseQuery
			.ApplySorting(filterRequest.SortBy, filterRequest.SortDirection)
			.Skip(filterRequest.Skip)
			.Take(filterRequest.PageSize)
			.Select(TDto.Selector())
			.ToListAsync(cancellationToken);

		return new PagedResult<TDto>(query, totalCount, filterRequest.Page, filterRequest.PageSize);
	}

	public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken) =>
		await DbSet.AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken);

	public virtual async Task<TDto?> GetDtoAsync<TDto>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken) where TDto : IBaseDto<T, TDto> =>
		await DbSet.AsNoTracking().Where(expression).Select(TDto.Selector()).FirstOrDefaultAsync(cancellationToken);

	public virtual async Task<IEnumerable<TDto>> GetListDtoAsync<TDto>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken) where TDto : IBaseDto<T, TDto> =>
		await DbSet.AsNoTracking().Where(expression).Select(TDto.Selector()).ToListAsync(cancellationToken);

	public virtual async Task<T?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken) =>
		await DbSet.FirstOrDefaultAsync(e => e.Guid == guid, cancellationToken);

	public virtual async Task<List<T>> ListAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken) =>
		await DbSet.AsNoTracking().Where(expression).ToListAsync(cancellationToken);

	public virtual async Task AddAsync(T entity, CancellationToken cancellationToken)
	{
		_ = await DbSet.AddAsync(entity, cancellationToken);
		_ = await AppDbContext.SaveChangesAsync(cancellationToken);
	}

	public virtual async Task AddRangeAsync(List<T> entities, CancellationToken cancellationToken)
	{
		await DbSet.AddRangeAsync(entities, cancellationToken);
		_ = await AppDbContext.SaveChangesAsync(cancellationToken);
	}

	public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken)
	{
		_ = DbSet.Update(entity);
		_ = await AppDbContext.SaveChangesAsync(cancellationToken);
	}

	public virtual async Task UpdateRangeAsync(List<T> entities, CancellationToken cancellationToken)
	{
		DbSet.UpdateRange(entities);
		_ = await AppDbContext.SaveChangesAsync(cancellationToken);
	}

	public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken)
	{
		try
		{
			_ = DbSet.Remove(entity);
			_ = await AppDbContext.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
		{
			var nomeEntidade = typeof(T).Name; // no projeto real: ToFriendlyEntityName()
			throw new CascadeDeleteException($"Este {nomeEntidade} não pode ser excluído pois existem registros vinculados a ele.");
		}
	}

	public virtual async Task DeleteRangeAsync(List<T> entities, CancellationToken cancellationToken)
	{
		DbSet.RemoveRange(entities);
		_ = await AppDbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken) =>
		await DbSet.AnyAsync(expression, cancellationToken);
}