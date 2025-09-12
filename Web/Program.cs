using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using meltix_web.Constantes;
using Microsoft.EntityFrameworkCore;
using Web.Middewares;

namespace meltix_web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;


            // Add services to the container.
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddSingleton<IFfMpegService, FfmpegService>();
            services.AddTransient<IThumbnailService, ThumbnailService>();
            services.AddTransient<IMediaInfoService, MediaInfoService>();
            services.AddHostedService<FolderScanService>();

            services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Adding auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            // Créer le dossier Data s'il n'existe pas
            var dataDir = Path.Combine(builder.Environment.ContentRootPath, "..", "Data");
            Directory.CreateDirectory(dataDir);

            var dbPath = Path.Combine(dataDir, "meltix.db");

            services.AddDbContext<MeltixContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));



            // App generation<
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {

                Directory.CreateDirectory(dataDir);

                // Créer une instance temporaire du DbContext avec SQLite
                var optionsBuilder = new DbContextOptionsBuilder<MeltixContext>();
                optionsBuilder.UseSqlite($"Data Source={dbPath}");

                using (var dbContext = new MeltixContext(optionsBuilder.Options))
                {
                    // Appliquer les migrations
                    dbContext.Database.Migrate();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(c => c
                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders(ApiConstantes.HeaderTotalCount)
                .AllowCredentials()
            );

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
