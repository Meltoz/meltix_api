using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class MeltixContext : DbContext
    {
        // DBSet
        public DbSet<Video> Videos { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

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

            modelBuilder.Entity<User>(builder =>
            {
                builder.OwnsOne(u => u.Pseudo, p =>
                {
                    p.Property(x => x.Value)
                    .HasColumnName("Pseudo")
                    .IsRequired();
                });

                builder.OwnsOne(u => u.Password, p =>
                {
                    p.Property(x => x.Value)
                    .HasColumnName("PasswordHash")
                    .IsRequired();
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
