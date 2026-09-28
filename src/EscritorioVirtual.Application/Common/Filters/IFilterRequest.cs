namespace EscritorioVirtual.Application.Common.Interfaces.Filters;

/// <summary>
/// Contrato base para qualquer requisição de listagem com filtro, ordenação e paginação.
/// </summary>
public interface IFilterRequest
{
	int Page { get; }
	int PageSize { get; }
	string? SortBy { get; }
	SortDirection SortDirection { get; }
}