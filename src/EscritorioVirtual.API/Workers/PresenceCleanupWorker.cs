using EscritorioVirtual.API.Hubs;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EscritorioVirtual.API.Workers;

public class PresenceCleanupWorker(
    IServiceScopeFactory scopeFactory,
    IHubContext<OfficeHub, IOfficeHubClient> hubContext,
    ILogger<PresenceCleanupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var staleList = await presenceService.GetStalePresencesAsync(15, stoppingToken);

                foreach (var stale in staleList)
                {
                    var elapsed = now - stale.LastHeartbeat;
                    if (elapsed >= 30)
                    {
                        // Excedeu o Grace Period total: desconecta do mapa definitivamente
                        await presenceService.RemovePresenceAsync(stale.UserId, stoppingToken);
                        await hubContext.Clients.Group($"map_{stale.MapId}").UserLeft(stale.UserId);
                        logger.LogInformation("Presença do usuário {UserId} expirou após {Elapsed}s no mapa {MapId}", stale.UserId, elapsed, stale.MapId);
                    }
                    else if (stale.Status != "reconnecting")
                    {
                        // Entrou no Grace Period (15s a 30s): marca como 'reconnecting'
                        await presenceService.UpdateStatusAsync(stale.UserId, "reconnecting", stoppingToken);
                        await hubContext.Clients.Group($"map_{stale.MapId}").StatusChanged(stale.UserId, "reconnecting");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar limpeza de presença de usuários inativos");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
