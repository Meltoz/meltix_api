using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    public class FolderScanService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FolderScanService(IServiceScopeFactory sf
        {
            _scopeFactory = sf;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
