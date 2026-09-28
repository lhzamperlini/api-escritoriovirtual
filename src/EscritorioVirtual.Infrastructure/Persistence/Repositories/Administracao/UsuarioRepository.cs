using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories.Administracao;

public class UsuarioRepository(AppDbContext appDbContext, ICurrentUserService currentUserService)
    : BaseRepository<Usuario>(appDbContext), IUsuarioRepository
{
    public async Task<Usuario?> ObterUsuarioAutenticadoAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            return null;
        }

        return await DbSet.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
    }

    public async Task<Usuario> SincronizarUsuarioAsync(
        Guid id,
        string email,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        var user = await DbSet.FindAsync([id], cancellationToken);
        if (user is null)
        {
            user = new Usuario(id, email, fullName);
            await DbSet.AddAsync(user, cancellationToken);
        }
        else
        {
            user.UpdateDetails(email, fullName);
            user.UpdateLastLogin();
            DbSet.Update(user);
        }

        await AppDbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
