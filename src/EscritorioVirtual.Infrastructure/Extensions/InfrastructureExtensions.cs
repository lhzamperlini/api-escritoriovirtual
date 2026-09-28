using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using EscritorioVirtual.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EscritorioVirtual.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services) =>
        services
            .AddHttpContextAccessor()
            .InjectRepositories()
            .InjectInfrastructureServices();

    public static IServiceCollection InjectRepositories(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssemblyOf<AppDbContext>()
            .AddClasses(classes => classes.AssignableTo(typeof(BaseRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

    public static IServiceCollection InjectInfrastructureServices(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssemblyOf<AppDbContext>()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Context") && t != typeof(AppDbContext)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
}
