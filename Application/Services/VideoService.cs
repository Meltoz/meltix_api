using Application.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;
using Shared.Enums.Sorting.Video;
using Shared.Exceptions;
using System.Threading.Channels;

namespace Application.Services
{
    public class VideoService
    {
        private readonly IVideoRepository _videoRepo;
        private readonly IThumbnailService _thumbnailService;
        private readonly IMediaInfoService _mediaInfoService;
        private readonly ITagRepository _tagRepository;
        private readonly FileService _fileService;
        private readonly IMapper _mapper;
        private readonly string _pathToWatch;
        private readonly string _pathThumbnail;
        private readonly ILogger<VideoService> _logger;

        public VideoService(IThumbnailService ts, 
            IMediaInfoService ms, 
            IVideoRepository vr, 
            ITagRepository tr, 
            FileService fs,
            IMapper m, 
            ILogger<VideoService> l,
            IOptions<ConnectionConfiguration> cf)
        {
            _videoRepo = vr;
            _thumbnailService = ts;
            _mediaInfoService = ms;
            _tagRepository = tr;
            _fileService = fs;
            _mapper = m;
            _logger = l;
            _pathThumbnail = cf.Value.Thumbnails;
            _pathToWatch = cf.Value.VideosFolder;
        }

        public async Task<VideoDTO> FindBySlugAsync(string slug)
        {
            var video = await _videoRepo.GetBySlug(slug);
            if (video == null)
                throw new EntityNotFoundException($"Video with slug: '{slug}' not found");

            return _mapper.Map<VideoDTO>(video);
        }

        public async Task<PagedResult<VideoDTO>> PaginateAsync(int pageIndex, int pageSize, string search, SortOption<SortVideo> sortOption, SearchScopeVideo scope = SearchScopeVideo.All)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var r = await _videoRepo.Search(skip, pageSize, search, sortOption, scope);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(r.Data);
            return new PagedResult<VideoDTO> {
                Data= videos,
                TotalCount = r.TotalCount
            };
        }


        public async Task<PagedResult<VideoDTO>> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var video = _mapper.Map<Video>(videoReference);
            video.Id = videoReference.Id;

            var data = await _videoRepo.GetRecommendation(skip, pageSize, video);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(data.Data);

            return new PagedResult<VideoDTO>
            {
                Data = videos,
                TotalCount = data.TotalCount
            };
        }

        public async Task SyncFolderWithDatabaseAsync()
        {

            if (!Directory.Exists(_pathToWatch))
            {
                return;
            }
            _logger.LogInformation($"Vérification dans {_pathToWatch}");

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

            await _videoRepo.SaveAsync();

            var dbFileNames = filesInDb.Select(f => f.Path).ToHashSet();

            var filesToAdd = filesOnDisk.Where(f => !dbFileNames.Contains(f)).ToList();

            if (!filesToAdd.Any())
                return;

            const int batchSize = 20;
            const int maxParallelism = 10;

            var channel = Channel.CreateUnbounded<Video>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true
            });

            var consumer = Task.Run(async () =>
            {
                var batch = new List<Video>(batchSize);

                await foreach (var video in channel.Reader.ReadAllAsync())
                {
                    batch.Add(video);

                    if (batch.Count >= batchSize)
                    {
                        await _videoRepo.InsertRangeAsync(batch);
                        await _videoRepo.SaveAsync();
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    await _videoRepo.InsertRangeAsync(batch);
                    await _videoRepo.SaveAsync();
                }
            });

            await Parallel.ForEachAsync(filesToAdd, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelism
            }, async (file, ct) =>
            {
                var inputPath = Path.Combine(_pathToWatch, file);
                var ouputPath = Path.Combine(_pathThumbnail, $"{Path.GetFileNameWithoutExtension(file)}.jpg");

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var timeSpanThumbnail = TimeSpan.FromSeconds(1);

                var thumbnail = await _thumbnailService.GenerateThumbnailAsync(inputPath, ouputPath, timeSpanThumbnail, cts.Token);
                var mediaInfo = await _mediaInfoService.GetMediaInfoAsync(inputPath, cts.Token);

                var video = new Video(file, thumbnail, mediaInfo.Duration);
                await channel.Writer.WriteAsync(video);
            });

            channel.Writer.Complete();

            await consumer;
        }

        public async Task<VideoDTO> UpdateVideoAsync(UpdateVideoDTO videoDTO)
        {
            var videoEntity = await GetVideoEntityAsync(videoDTO.Id);
            var pathVideo = Path.Combine(_pathToWatch, videoEntity.Path);

            await UpdateThumbnailAsync(videoEntity, videoDTO, pathVideo);
            await UpdateTitleAndFileAsync(videoEntity, videoDTO.Title, pathVideo);
            await UpdateTagsAsync(videoEntity, videoDTO.Tags);
            UpdateBasicProperties(videoEntity, videoDTO);

            var video = await _videoRepo.UpdateAsync(videoEntity);
            return _mapper.Map<VideoDTO>(video);
        }

        private async Task<Video> GetVideoEntityAsync(Guid id)
        {
            var entity = await _videoRepo.GetByIdAsync(id);
            if (entity == null)
                throw new EntityNotFoundException($"Video with id '{id}' not found");
            return entity;
        }

        private async Task UpdateTitleAndFileAsync(Video entity, string newTitle, string currentPath)
        {
            if (entity.Title == newTitle) return;

            entity.ChangeTitle(newTitle);
            var extension = Path.GetExtension(currentPath);
            var newName = $"{entity.Slug}{extension}";
            _fileService.Rename(currentPath, newName);
            entity.ChangePath(newName);
        }

        private async Task UpdateThumbnailAsync(Video entity, UpdateVideoDTO dto, string videoPath)
        {
            if (dto.ThumbnailFile != null)
            {
                await UpdateThumbnailFromFileAsync(entity, dto.ThumbnailFile);
                return;
            }

            if (dto.Timecode != TimeSpan.Zero)
            {
                await GenerateThumbnailFromTimecodeAsync(entity, videoPath, dto.Timecode.Value);
            }
        }

        private async Task UpdateThumbnailFromFileAsync(Video entity, ThumbnailFileData file)
        {
            var outputPath = GetThumbnailPath(entity);
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            await file.Stream.CopyToAsync(fileStream);
            entity.ChangeThumbnail(outputPath);
        }

        private async Task GenerateThumbnailFromTimecodeAsync(Video entity, string videoPath, TimeSpan timecode)
        {
            if (!File.Exists(videoPath))
                throw new FileNotFoundException($"Video file not found: {videoPath}");

            var outputPath = GetThumbnailPath(entity);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            try
            {
                var thumbnailPath = await _thumbnailService.GenerateThumbnailAsync(
                    videoPath, outputPath, timecode, cts.Token);
                entity.ChangeThumbnail(thumbnailPath);
            }
            catch
            {
                CleanupThumbnailFile(outputPath);
                throw;
            }
        }

        private void CleanupThumbnailFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private string GetThumbnailPath(Video entity)
        {
            return Path.Combine(_pathThumbnail,
                $"{Path.GetFileNameWithoutExtension(entity.Path)}.jpg");
        }

        private async Task UpdateTagsAsync(Video entity, IEnumerable<string> newTags)
        {
            var dtoTags = new HashSet<string>(newTags.Select(t => t.Trim().ToLower()));
            var entityTags = new HashSet<string>(entity.Tags.Select(t => t.Value));

            var tagsToAdd = dtoTags.Except(entityTags);
            var tagsToRemove = entityTags.Except(dtoTags);

            await AddTagsAsync(entity, tagsToAdd);
            RemoveTags(entity, tagsToRemove);
        }

        private async Task AddTagsAsync(Video entity, IEnumerable<string> tagValues)
        {
            foreach (var tagValue in tagValues)
            {
                var tag = await _tagRepository.GetByNameAsync(tagValue)
                    ?? new Tag(tagValue);
                entity.AddTags(tag);
            }
        }

        private void RemoveTags(Video entity, IEnumerable<string> tagValues)
        {
            foreach (var tagValue in tagValues)
            {
                var tag = entity.Tags.First(t => t.Value == tagValue);
                entity.RemoveTags(tag);
            }
        }

        private void UpdateBasicProperties(Video entity, UpdateVideoDTO dto)
        {
            entity.Description = dto.Description;
            entity.CategoryId = dto.Category.Id;
        }
    }
}
