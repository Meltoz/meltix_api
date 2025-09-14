using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics;


namespace Infrastructure.Data
{
    public class MeltixContext : DbContext
    {
        // DBSet
        public DbSet<Video> Videos { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public MeltixContext()
        {

        }

        public MeltixContext(DbContextOptions<MeltixContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>()
                .HasMany(v => v.Tags)
                .WithMany(c => c.Videos)
                .UsingEntity(v => v.ToTable("VideoTags"));

            modelBuilder.Entity<Video>()
                .HasOne(v => v.Category)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Tag>(t => t.HasIndex(t2 => t2.Value)
            .IsUnique());

            base.OnModelCreating(modelBuilder);
        }
    }
}
