using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data.Repositories;
using Meltoz_infrastructure.Data;
using Microsoft.EntityFrameworkCore.Design;
using Shared;

namespace Application.Services
{
    public class VideoService : IVideoService
    {
        public readonly VideoRepository _videoRepo;
        

        public VideoService(MeltixContext c)
        {
            _videoRepo = new VideoRepository(c);            
        }
    }
}
