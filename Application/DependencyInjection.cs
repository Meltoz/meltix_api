using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<VideoService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<TagService>();
            services.AddScoped<UserService>();
            services.AddScoped<AesEncryptionService>();
            services.AddScoped<TokenService>();
            services.AddSingleton<IFfMpegService, FfmpegService>();
            services.AddTransient<IThumbnailService, ThumbnailService>();
            services.AddTransient<IMediaInfoService, MediaInfoService>();
            services.AddTransient<JwtService>();
            services.AddHostedService<FolderScanService>(); 

            services.AddAutoMapper(cfg => { }, typeof(DtoToEntitiesProfile), typeof(EntitiesToDtoProfile));

            return services;
        }

    }
}
