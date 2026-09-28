using EscritorioVirtual.Application.Common.Interfaces.Repositories;
using EscritorioVirtual.Domain.AggregateRoot;

namespace EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> ObterUsuarioAutenticadoAsync(CancellationToken cancellationToken = default);
    Task<Usuario> SincronizarUsuarioAsync(Guid id, string email, string fullName, CancellationToken cancellationToken = default);
}