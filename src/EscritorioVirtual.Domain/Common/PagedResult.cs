namespace EscritorioVirtual.Domain.Common;
/// <summary>
/// Resultado paginado genérico retornado por qualquer query de listagem.
/// </summary>
public sealed class PagedResult<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
{
	public IReadOnlyList<T> Items { get; } = items;
	public int TotalCount { get; } = totalCount;
	public int Page { get; } = page;
	public int PageSize { get; } = pageSize;
	public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);


#pragma warning disable CA1000
	public static PagedResult<T> Empty => new(Array.Empty<T>(), 0, 1, 10);
#pragma warning restore CA1000
}