using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    public class FolderScanService : BackgroundService
    {
        private readonly IVideoService _videoService;

        public FolderScanService(IServiceScopeFactory sf)
        {
            var scope = sf.CreateScope();
            _videoService = scope.ServiceProvider.GetService<IVideoService>();


        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _videoService.SyncFolderWithDatabaseAsync();
                }
                catch (Exception ex)
                {
                    
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

    }
}
