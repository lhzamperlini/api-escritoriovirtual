using System.Linq.Expressions;
using System.Reflection;
using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Infrastructure.Extensions;

/// <summary>
/// Extensões sobre IQueryable para aplicar ordenação e paginação de forma dinâmica
/// a partir de uma FilterRequest.
/// </summary>
public static class QueryableExtensions
{
	public static IQueryable<T> ApplySorting<T>(
			this IQueryable<T> query,
			string? sortBy,
			SortDirection direction)
	{
		if (string.IsNullOrWhiteSpace(sortBy))
			return query;

		var property = typeof(T)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(p =>
						string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase));

		if (property is null)
			return query;

		var param = Expression.Parameter(typeof(T), "x");
		var memberAccess = Expression.Property(param, property);
		// Converte para object para compatibilidade genérica
		var converted = Expression.Convert(memberAccess, typeof(object));
		var keySelector = Expression.Lambda<Func<T, object>>(converted, param);

		return direction == SortDirection.Desc
				? query.OrderByDescending(keySelector)
				: query.OrderBy(keySelector);
	}
}