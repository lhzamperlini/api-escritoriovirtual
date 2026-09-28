using FluentValidation;
using EscritorioVirtual.Application.Common.Behaviors;
using EscritorioVirtual.Application.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EscritorioVirtual.Application.DependencyInjection;
public static class ApplicationExtensions
{
	public static IServiceCollection ConfigureMediatr(this IServiceCollection services)
	{
		_ = services.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly);
		_ = services.AddMediatR(cfg =>
		{
			_ = cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly);
			_ = cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
		});

		_ = services.InjectServices();

		return services;
	}

	/// <summary>
	/// Registra automaticamente todos os services cujas interfaces herdam de <see cref="IBaseService"/>
	/// no contêiner de injeção de dependência, mapeando-os para suas interfaces correspondentes.
	/// </summary>
	public static IServiceCollection InjectServices(this IServiceCollection services)
	{
		_ = services.Scan(scan => scan
			.FromAssemblyOf<IBaseService>()
			.AddClasses(classes => classes.AssignableTo<IBaseService>())
			.AsImplementedInterfaces(service => service != typeof(IBaseService))
			.WithScopedLifetime());

		return services;
	}
}