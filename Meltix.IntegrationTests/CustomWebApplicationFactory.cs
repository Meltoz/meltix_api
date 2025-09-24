using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web;

namespace Meltix.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly MeltixContext _context;

        public CustomWebApplicationFactory(MeltixContext context)
        {
            _context = context;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Supprime l'ancien DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MeltixContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<MeltixContext>(opt =>
                {
                    opt.UseSqlite(_context.Database.GetDbConnection());
                });
            });
        }
    }
}
