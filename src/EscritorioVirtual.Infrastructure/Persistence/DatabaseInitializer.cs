using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EscritorioVirtual.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabaseAndTablesCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetService<ILogger<AppDbContext>>();

        try
        {
            var creator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (!await creator.ExistsAsync())
            {
                await creator.CreateAsync();
                logger?.LogInformation("Banco de dados criado com sucesso.");
            }

            var hasTables = await creator.HasTablesAsync();
            if (!hasTables)
            {
                await creator.CreateTablesAsync();
                logger?.LogInformation("Tabelas do modelo EF Core criadas com sucesso.");
            }
            else
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger?.LogInformation("Aplicando migrations pendentes...");
                    await context.Database.MigrateAsync();
                    logger?.LogInformation("Migrations aplicadas com sucesso.");
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Aviso na inicialização automática do banco/tabelas: {Message}", ex.Message);
        }
    }
}
