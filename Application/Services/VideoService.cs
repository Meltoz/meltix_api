using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Shared;
using Shared.Enums;
using Shared.Exceptions;
using System.Threading.Channels;

namespace Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly VideoRepository _videoRepo;
        private readonly IThumbnailService _thumbnailService;
        private readonly IMediaInfoService _mediaInfoService;
        private readonly TagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly string _pathToWatch = @"E:\ToDelete";


        public VideoService(MeltixContext c, IThumbnailService ts, IMediaInfoService ms, IMapper m)
        {
            _videoRepo = new VideoRepository(c);
            _thumbnailService = ts;
            _mediaInfoService = ms;
            _tagRepository = new TagRepository(c);
            _mapper = m;
        }

        public async Task<VideoDTO> FindBySlugAsync(string slug)
        {
            var video = await _videoRepo.GetBySlug(slug);
            if (video == null)
                throw new EntityNotFoundException($"Video with slug: '{slug}' not found");

            return _mapper.Map<VideoDTO>(video);
        }

        public async Task<PagedResult<VideoDTO>> GetLastestVideos(int pageIndex, int pageSize, int days)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var lastestVideos = await _videoRepo.GetLatest(skip, pageSize, days);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(lastestVideos.videos);
            return new PagedResult<VideoDTO> { 
                Data = videos, 
                TotalCount = lastestVideos.totalCount 
            };
        }

        public async Task<PagedResult<VideoDTO>> PaginateAsync(int pageIndex, int pageSize, string search, SearchScopeVideo scope = SearchScopeVideo.All)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var r = await _videoRepo.Search(skip, pageSize, search, scope);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(r.videos);
            return new PagedResult<VideoDTO> {
                Data= videos,
                TotalCount = r.totalCount
            };
        }


        public async Task<PagedResult<VideoDTO>> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var video = _mapper.Map<Video>(videoReference);
            video.Id = videoReference.Id;

            var data = await _videoRepo.GetRecommendation(skip, pageSize, video);

            var videos = _mapper.Map<IEnumerable<VideoDTO>>(data.videos);

            return new PagedResult<VideoDTO>
            {
                Data = videos,
                TotalCount = data.totalCount
            };
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
                        await _videoRepo.InsertRangeAsync(batch.ToArray());
                        await _videoRepo.SaveAsync();
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    await _videoRepo.InsertRangeAsync(batch.ToArray());
                    await _videoRepo.SaveAsync();
                }
            });

            await Parallel.ForEachAsync(filesToAdd, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelism
            }, async (file, ct) =>
            {
                var inputPath = Path.Combine(_pathToWatch, file);
                var endPath = $@"Data\Thumbnails\{Path.GetFileNameWithoutExtension(file)}.jpg";
                var ouputPath = Path.Combine(AppContext.BaseDirectory, @"..", "..", "..", "..", endPath);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                var thumbnail = await _thumbnailService.GenerateThumbnailAsync(inputPath, ouputPath, cts.Token);
                var mediaInfo = await _mediaInfoService.GetMediaInfoAsync(inputPath, cts.Token);

                var video = new Video(file, thumbnail, mediaInfo.Duration);
                await channel.Writer.WriteAsync(video);
            });

            channel.Writer.Complete();

            await consumer;
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

            var dtoTags = new HashSet<string>(videoDTO.Tags);                
            var entityTags = new HashSet<string>(videoEntity.Tags.Select(t => t.Value));

            var tagsToAdd = dtoTags.Except(entityTags);

            var tagsToRemove = entityTags.Except(dtoTags);

            foreach (var tagValue in tagsToAdd)
            {
                var tagFind= await _tagRepository.GetByNameAsync(tagValue);
                var t = tagFind != null ? tagFind : new Tag(tagValue);

                videoEntity.AddTags(t);
            }
            foreach (var tagValue in tagsToRemove)
            {
                var tag = videoEntity.Tags.First(t => t.Value == tagValue);
                videoEntity.RemoveTags(tag);
            }
            videoEntity.Description = videoDTO.Description;
            videoEntity.CategoryId = videoDTO.Category.Id;
            var video = await _videoRepo.UpdateAsync(videoEntity);

            return _mapper.Map<VideoDTO>(video);
        }
    }
}
