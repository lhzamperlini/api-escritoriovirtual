using EscritorioVirtual.Application.Common.Interfaces.Repositories;
using EscritorioVirtual.Domain.AggregateRoot;

namespace EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
public interface IUsuarioRepository : IBaseRepository<Usuario>
{
	Task<Usuario?> ObterUsuarioAutenticado(CancellationToken cancellationToken);
}