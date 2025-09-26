using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddSingleton<IFfMpegService, FfmpegService>();
            services.AddTransient<IThumbnailService, ThumbnailService>();
            services.AddTransient<IMediaInfoService, MediaInfoService>();
            services.AddHostedService<FolderScanService>();

            services.AddAutoMapper(cfg => { }, typeof(DtoToEntitiesProfile), typeof(EntitiesToDtoProfile));

            return services;
        }

    }
}
