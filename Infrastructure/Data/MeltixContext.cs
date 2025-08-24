using Meltix_domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace Meltoz_infrastructure.Data
{
    public class MeltixContext : DbContext
    {
        // DBSet
        public DbSet<Video> Videos { get; set; }

        public DbSet<Category> Categories { get; set; }


        public MeltixContext()
        {
            
        }

        public MeltixContext(DbContextOptions<MeltixContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        private string GetSolutionDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();

            // Chercher le fichier .sln en remontant dans l'arborescence
            while (currentDir != null && !Directory.GetFiles(currentDir, "*.sln").Any())
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            return currentDir ?? throw new DirectoryNotFoundException("Solution directory not found");
        }

    }
}
