using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Application.Avatars.Dtos;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Domain.Aggregates.Avatars;
using MediatR;

namespace EscritorioVirtual.Application.Avatars.Queries.GetMyAvatar;

public record GetMyAvatarQuery : IRequest<AvatarConfigDto>;

public class GetMyAvatarQueryHandler(
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyAvatarQuery, AvatarConfigDto>
{
    public async Task<AvatarConfigDto> Handle(GetMyAvatarQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Usuário não autenticado.");
        }

        var usuario = await usuarioRepository.ObterUsuarioAutenticadoAsync(cancellationToken);
        if (usuario is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        var config = AvatarCustomization.FromJson(usuario.AvatarConfig);
        return MapToDto(config);
    }

    public static AvatarConfigDto MapToDto(AvatarCustomization config)
    {
        return new AvatarConfigDto
        {
            Base = new AvatarPartDto(config.Base.AssetId, config.Base.Tint),
            Hair = new AvatarPartDto(config.Hair.AssetId, config.Hair.Tint),
            Eyes = new AvatarPartDto(config.Eyes.AssetId, config.Eyes.Tint),
            Top = new AvatarPartDto(config.Top.AssetId, config.Top.Tint),
            Bottom = new AvatarPartDto(config.Bottom.AssetId, config.Bottom.Tint),
            Shoes = new AvatarPartDto(config.Shoes.AssetId, config.Shoes.Tint),
            Accessories = config.Accessories
                .Select(a => new AvatarPartDto(a.AssetId, a.Tint))
                .ToList()
        };
    }
}
