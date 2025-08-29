using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<MeltixContext>
    {
        public MeltixContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Web");
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MeltixContext>();
            optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));

            return new MeltixContext(optionsBuilder.Options);
        }
    }
}
