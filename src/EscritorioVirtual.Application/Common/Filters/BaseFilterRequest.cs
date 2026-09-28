using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using EscritorioVirtual.Application.Common.Interfaces.Filters;

namespace EscritorioVirtual.Application.Common.Filters;

/// <summary>
/// Classe base que toda request de listagem deve herdar.
/// Contém paginação e ordenação. Filtros específicos ficam nas subclasses.
/// </summary>
public abstract class BaseFilterRequest<T, TDto> : IFilterRequest where T : BaseEntity
{
	private const int MaxPageSize = 100;
	private const int DefaultPageSize = 20;

	private int _pageSize = DefaultPageSize;
	private int _page = 1;

	[Range(1, int.MaxValue, ErrorMessage = "Page deve ser maior que 0.")]
	public int Page
	{
		get => _page;
		init => _page = value < 1 ? 1 : value;
	}

	[Range(1, 100, ErrorMessage = "PageSize deve ser entre 1 e 100.")]
	public virtual int PageSize
	{
		get => _pageSize;
		init => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
	}

	public virtual string? SortBy { get; init; } = "Id";

	public virtual SortDirection SortDirection { get; init; } = SortDirection.Desc;

	public int Skip => (Page - 1) * PageSize;


	/// <summary>
	///   Método para montar a expressão pro Count e pro Where
	/// </summary>
	public abstract Expression<Func<T, bool>> Expression();
}