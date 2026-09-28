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

            try
            {
                await creator.CreateTablesAsync();
                logger?.LogInformation("Tabelas do modelo EF Core criadas com sucesso.");
            }
            catch
            {
                // Se algumas tabelas já existem no banco, aplica a criação idempotente de tabelas faltantes
                var ddlScript = context.Database.GenerateCreateScript();
                var safeScript = ddlScript
                    .Replace("CREATE TABLE ", "CREATE TABLE IF NOT EXISTS ")
                    .Replace("CREATE INDEX ", "CREATE INDEX IF NOT EXISTS ")
                    .Replace("CREATE UNIQUE INDEX ", "CREATE UNIQUE INDEX IF NOT EXISTS ");

                await context.Database.ExecuteSqlRawAsync(safeScript);
                logger?.LogInformation("Tabelas e índices verificados e criados com sucesso de forma idempotente.");
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Aviso na inicialização automática do banco/tabelas: {Message}", ex.Message);
        }
    }
}
