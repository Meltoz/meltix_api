using Application.Interfaces.Services;
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
            services.AddSingleton<IFfMpegService, FfmpegService>();
            services.AddTransient<IThumbnailService, ThumbnailService>();
            services.AddTransient<IMediaInfoService, MediaInfoService>();
            services.AddHostedService<FolderScanService>();

            return services;
        }

    }
}
