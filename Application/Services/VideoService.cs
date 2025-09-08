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
        private readonly IMapper _mapper;
        private readonly string _pathToWatch = @"E:\ToDelete";


        public VideoService(MeltixContext c, IMapper m)
        {
            _videoRepo = new VideoRepository(c);
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
                var videoInfo = await GetVideoInfo(file);
                var video = new Video(file, videoInfo.ThumbnailPath, videoInfo.Duration);
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

        private async Task<VideoInfoResult> GetVideoInfo(string videoPath)
        {
            var videoInfo = new VideoInfoResult();
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            var pathInput = Path.Combine(_pathToWatch, videoPath);
            var mediaInfo = await FFmpeg.GetMediaInfo(pathInput);
            var baseDirectoryPath = AppContext.BaseDirectory;
            var endPath = $@"Data\\Thumbnails\\{videoPath.Substring(0, videoPath.Length - 4)}.jpg";
            var ouputPath = Path.Combine(baseDirectoryPath, @$"..", "..", "..", "..", endPath);
            var videoStream = mediaInfo.VideoStreams.FirstOrDefault();

            var conversion = FFmpeg.Conversions.New()
                     .AddStream(videoStream)
                    .SetSeek(TimeSpan.FromSeconds(1))
                    .AddParameter("-vframes 1")
                    .SetOutput(ouputPath)
                    .Start();

            videoInfo.Duration = (int)mediaInfo.Duration.TotalSeconds;
            videoInfo.ThumbnailPath = endPath;
            return videoInfo;
        }
    }

    internal class VideoInfoResult
    {
        public string ThumbnailPath { get; set; }

        public int Duration { get; set; }
    }
}
