using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class MeltixContext : DbContext
    {
        private readonly AesEncryptionService _encryptionService;

        // DBSet
        public DbSet<Video> Videos { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<TokenInfo> Tokens { get; set; }

        public MeltixContext()
        {

        }

        public MeltixContext(DbContextOptions<MeltixContext> options, AesEncryptionService es) : base(options)
        {
            _encryptionService = es;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tokenConverter = new TokenValueConverter(_encryptionService, null);

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
                builder.ComplexProperty(u => u.Pseudo, p =>
                {
                    p.Property(x => x.Value)
                        .HasColumnName("Pseudo")
                        .IsRequired();
                });


                builder.ComplexProperty(u => u.Password, p =>
                {
                    p.Property(x => x.Value)
                        .HasColumnName("Password")
                        .IsRequired();
                });
            });

            modelBuilder.Entity<TokenInfo>(builder =>
            {
                builder.HasKey(t => t.Id);

                builder.Property(t => t.AccessToken)
                .HasConversion(tokenConverter);

                builder.Property(t => t.RefreshToken)
                .HasConversion(tokenConverter);

                builder
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
