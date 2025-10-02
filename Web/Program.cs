using Application;
using Application.Mappings;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Web.Constantes;
using Web.Mappings;
using Web.Middewares;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            services.Configure<AuthConfiguration>(builder.Configuration.GetSection("Auth"));
            services.Configure<FfmpegConfiguration>(builder.Configuration.GetSection("FFmpeg"));

            services.AddResponseCaching();

            // Add services to the container.
            services.AddApplication();

            services.AddInfrastructure(builder.Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Auth:Issuer"],
                        ValidAudience = builder.Configuration["Auth:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Auth:Key"]))
                    };
                });

            services.AddAuthorization();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAutoMapper(cfg => { }, typeof(DtoToViewModelProfile), typeof(ViewModelToDtoProfile));

            // Cr�er le dossier Data s'il n'existe pas
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

                // Cr�er une instance temporaire du DbContext avec SQLite
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
