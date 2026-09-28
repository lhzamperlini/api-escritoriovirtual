using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Avatars.Dtos;
using EscritorioVirtual.Application.Avatars.Queries.GetMyAvatar;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Domain.Aggregates.Avatars;
using MediatR;

namespace EscritorioVirtual.Application.Avatars.Queries.GetUserAvatar;

public record GetUserAvatarQuery(Guid UserId) : IRequest<AvatarConfigDto>;

public class GetUserAvatarQueryHandler(IUsuarioRepository usuarioRepository)
    : IRequestHandler<GetUserAvatarQuery, AvatarConfigDto>
{
    public async Task<AvatarConfigDto> Handle(GetUserAvatarQuery request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetAsync(u => u.Id == request.UserId, cancellationToken);
        if (usuario is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        var config = AvatarCustomization.FromJson(usuario.AvatarConfig);
        return GetMyAvatarQueryHandler.MapToDto(config);
    }
}
