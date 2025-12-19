using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FolderScanService(IServiceScopeFactory sf) : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = sf;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var videoService = scope.ServiceProvider.GetRequiredService<VideoService>();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FolderScanService>>();

                        logger.LogInformation("Début du scan des vidéos");
                        await videoService.SyncFolderWithDatabaseAsync();
                        logger.LogInformation("Scan des vidéos terminé");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du scan : {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

    }
}
