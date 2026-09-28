using System.Linq.Expressions;

namespace EscritorioVirtual.Application.Common.Interfaces.Dtos;
public interface IBaseDto<T, TDto> where T : BaseEntity where TDto : IBaseDto<T, TDto>
{
	static abstract Expression<Func<T, TDto>> Selector();

	/// <summary>
	/// Converte, em memória, uma entidade já carregada para o Dto correspondente, reaproveitando a mesma projeção
	/// utilizada nas consultas (<see cref="Selector"/>), sem a necessidade de uma nova consulta ao banco de dados.
	/// </summary>
#pragma warning disable CA1000
	static TDto ToDto(T entidade) => TDto.Selector().Compile().Invoke(entidade);
#pragma warning restore CA1000
}