using System.Text.RegularExpressions;
using FluentValidation;
using EscritorioVirtual.Application.Avatars.Dtos;
using EscritorioVirtual.Application.Common.Exceptions;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using EscritorioVirtual.Domain.Aggregates.Avatars;
using MediatR;

namespace EscritorioVirtual.Application.Avatars.Commands.UpdateAvatar;

public record UpdateAvatarCommand(AvatarConfigDto AvatarConfig) : IRequest<AvatarConfigDto>;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    private static readonly Regex HexColorRegex = new(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);

    public UpdateAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarConfig)
            .NotNull().WithMessage("A configuração do avatar é obrigatória.");

        When(x => x.AvatarConfig != null, () =>
        {
            RuleFor(x => x.AvatarConfig.Base)
                .NotNull().WithMessage("A base do avatar é obrigatória.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Base.AssetId).NotEmpty().WithMessage("O asset da base é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Base.Tint).Must(BeValidHexColor).WithMessage("O tint da base deve ser um código hexadecimal válido.");
                });

            RuleFor(x => x.AvatarConfig.Hair)
                .NotNull().WithMessage("O cabelo do avatar é obrigatório.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Hair.AssetId).NotEmpty().WithMessage("O asset do cabelo é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Hair.Tint).Must(BeValidHexColor).WithMessage("O tint do cabelo deve ser um código hexadecimal válido.");
                });

            RuleFor(x => x.AvatarConfig.Eyes)
                .NotNull().WithMessage("Os olhos do avatar são obrigatórios.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Eyes.AssetId).NotEmpty().WithMessage("O asset dos olhos é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Eyes.Tint).Must(BeValidHexColor).WithMessage("O tint dos olhos deve ser um código hexadecimal válido.");
                });

            RuleFor(x => x.AvatarConfig.Top)
                .NotNull().WithMessage("A parte de cima (blusa) é obrigatória.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Top.AssetId).NotEmpty().WithMessage("O asset da blusa é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Top.Tint).Must(BeValidHexColor).WithMessage("O tint da blusa deve ser um código hexadecimal válido.");
                });

            RuleFor(x => x.AvatarConfig.Bottom)
                .NotNull().WithMessage("A parte de baixo (calça) é obrigatória.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Bottom.AssetId).NotEmpty().WithMessage("O asset da calça é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Bottom.Tint).Must(BeValidHexColor).WithMessage("O tint da calça deve ser um código hexadecimal válido.");
                });

            RuleFor(x => x.AvatarConfig.Shoes)
                .NotNull().WithMessage("Os sapatos são obrigatórios.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.AvatarConfig.Shoes.AssetId).NotEmpty().WithMessage("O asset dos sapatos é obrigatório.");
                    RuleFor(x => x.AvatarConfig.Shoes.Tint).Must(BeValidHexColor).WithMessage("O tint dos sapatos deve ser um código hexadecimal válido.");
                });

            RuleForEach(x => x.AvatarConfig.Accessories).ChildRules(accessory =>
            {
                accessory.RuleFor(a => a.AssetId).NotEmpty().WithMessage("O asset do acessório é obrigatório.");
                accessory.RuleFor(a => a.Tint).Must(BeValidHexColor).WithMessage("O tint do acessório deve ser um código hexadecimal válido.");
            });
        });
    }

    private static bool BeValidHexColor(string? color)
    {
        if (string.IsNullOrWhiteSpace(color)) return false;
        return HexColorRegex.IsMatch(color);
    }
}

public class UpdateAvatarCommandHandler(
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateAvatarCommand, AvatarConfigDto>
{
    public async Task<AvatarConfigDto> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
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

        var domainConfig = new AvatarCustomization
        {
            Base = new AvatarPartConfig(request.AvatarConfig.Base.AssetId, request.AvatarConfig.Base.Tint),
            Hair = new AvatarPartConfig(request.AvatarConfig.Hair.AssetId, request.AvatarConfig.Hair.Tint),
            Eyes = new AvatarPartConfig(request.AvatarConfig.Eyes.AssetId, request.AvatarConfig.Eyes.Tint),
            Top = new AvatarPartConfig(request.AvatarConfig.Top.AssetId, request.AvatarConfig.Top.Tint),
            Bottom = new AvatarPartConfig(request.AvatarConfig.Bottom.AssetId, request.AvatarConfig.Bottom.Tint),
            Shoes = new AvatarPartConfig(request.AvatarConfig.Shoes.AssetId, request.AvatarConfig.Shoes.Tint),
            Accessories = request.AvatarConfig.Accessories
                .Select(a => new AvatarPartConfig(a.AssetId, a.Tint))
                .ToList()
        };

        usuario.UpdateAvatar(domainConfig.ToJson());
        await usuarioRepository.UpdateAsync(usuario, cancellationToken);

        return request.AvatarConfig;
    }
}
