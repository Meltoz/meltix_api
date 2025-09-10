using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Shared.Exceptions;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly VideoRepository _videoRepo;
        private readonly IThumbnailService _thumbnailService;
        private readonly IMediaInfoService _mediaInfoService;
        private readonly IMapper _mapper;
        private readonly string _pathToWatch = @"E:\ToDelete";


        public VideoService(MeltixContext c, IThumbnailService ts, IMediaInfoService ms, IMapper m)
        {
            _videoRepo = new VideoRepository(c);
            _thumbnailService = ts;
            _mediaInfoService = ms;
            _mapper = m;
        }

        public async Task<VideoDTO> FindBySlugAsync(string slug)
        {
            var video = await _videoRepo.GetBySlug(slug);
            if (video == null)
                throw new EntityNotFoundException($"Video with slug: '{slug}' not found");

            return _mapper.Map<VideoDTO>(video);
        }

        public async Task<(IEnumerable<VideoDTO> videos, int totalCount)> PaginateAsync(int pageIndex, int pageSize, string search)
        {

            var skip = pageIndex < 0 ? 0 : pageIndex * pageSize;

            var r = await _videoRepo.Search(skip, pageSize, search);

            return (
                _mapper.Map<IEnumerable<VideoDTO>>(r.videos),
                r.totalCount);
        }

        public async Task<(IEnumerable<VideoDTO> videos, int totalCount)> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference)
        {
            var skip = pageIndex < 0 ? 0 : pageIndex * pageSize;

            var video = _mapper.Map<Video>(videoReference);
            video.Id = videoReference.Id;

            var data = await _videoRepo.GetRecommendation(skip, pageSize, video);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(data.videos);
            return (videos, data.totalCount);
        }

        public async Task SyncFolderWithDatabaseAsync()
        {

            if (!Directory.Exists(_pathToWatch))
            {
                return;
            }

            var filesOnDisk = Directory.GetFiles(_pathToWatch)
                                        .Where(f => f.EndsWith(".mp4"))
                                       .Select(Path.GetFileName)
                                       .ToHashSet();

            var filesInDb = (await _videoRepo.GetAllAsync()).ToList();

            // 1. Supprimer de la BDD les fichiers qui n'existent plus
            var toDelete = filesInDb.Where(f => !filesOnDisk.Contains(f.Path)).ToList();
            if (toDelete.Any())
            {
                foreach (var video in toDelete)
                {
                    _videoRepo.Delete(video.Id);
                }
            }

            // 2. Ajouter les fichiers du disque qui ne sont pas en BDD
            var dbFileNames = filesInDb.Select(f => f.Path).ToHashSet();

            var filesToAdd = filesOnDisk.Where(f => !dbFileNames.Contains(f)).ToList();

            foreach (var file in filesToAdd)
            {
                var inputPath = Path.Combine(_pathToWatch, file);
                var endPath = $@"Data\Thumbnails\{file.Substring(0, file.Length - 4)}.jpg";
                var ouputPath = Path.Combine(AppContext.BaseDirectory, @"..", "..", "..", "..", endPath);
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                var thumbnail = await _thumbnailService.GenerateThumbnailAsync(inputPath, ouputPath, cts.Token);
                var mediaInfo = await _mediaInfoService.GetMediaInfoAsync(inputPath, cts.Token);

                var video = new Video(file, thumbnail, mediaInfo.Duration);
                await _videoRepo.InsertWithOutSave(video);
            }

            await _videoRepo.SaveAsync();
        }

        public async Task<VideoDTO> UpdateVideoAsync(VideoDTO videoDTO)
        {
            var videoEntity = await _videoRepo.GetByIdAsync(videoDTO.Id);
            if (videoEntity == null)
                throw new EntityNotFoundException($"Video with id '{videoDTO.Id}' not found");

            if (videoEntity.Title != videoDTO.Title)
            {
                videoEntity.ChangeTitle(videoDTO.Title);
            }

            videoEntity.Description = videoDTO.Description;
            videoEntity.CategoryId = videoDTO.Category.Id;
            var video = await _videoRepo.UpdateAsync(videoEntity);

            return _mapper.Map<VideoDTO>(video);
        }
    }
}
