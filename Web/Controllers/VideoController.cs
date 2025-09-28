using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using meltix_web.Constantes;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting.Video;
using System.Text.RegularExpressions;
using Web.Constantes;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;
        private readonly CategoryService _categoryService;
        private readonly IMapper _mapper;

        public VideoController(VideoService vs, CategoryService cs, IMapper m)
        {
            _videoService = vs;
            _categoryService = cs;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos(int pageIndex, int pageSize, string sort, string? search)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            if (!GetSorting(sort, out var sortOptions))
                return BadRequest();

            var videos = await _videoService.PaginateAsync(pageIndex, pageSize, search ?? "", sortOptions);

            IEnumerable<VideoCardVM> videoVM = _mapper.Map<IEnumerable<VideoCardVM>>(videos.Data);

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, videos.TotalCount.ToString());
            return Ok(videoVM);
        }

        // ici on va pouvoir mettre un authorize sur la methode
        [HttpGet]
        public async Task<IActionResult> GetUnCategorisedVideos(int pageIndex, int pageSize, string sort, string? search)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            if (!GetSorting(sort, out var sortOptions))
                return BadRequest();


            var videos = await _videoService.PaginateAsync(pageIndex, pageSize, search ?? "", sortOptions, SearchScopeVideo.Uncategorised);

            IEnumerable<VideoCardVM> videoVM = _mapper.Map<IEnumerable<VideoCardVM>>(videos.Data);

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, videos.TotalCount.ToString());
            return Ok(videoVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(string slug)
        {
            if (string.IsNullOrEmpty(slug.Trim()))
                return BadRequest(ModelState);

            var videoDTO = await _videoService.FindBySlugAsync(slug);

            return Ok(_mapper.Map<VideoVM>(videoDTO));
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            await _videoService.SyncFolderWithDatabaseAsync();

            return Ok();
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateVideo([FromForm] VideoRequestVM video)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            UpdateVideoDTO videoDTO = _mapper.Map<UpdateVideoDTO>(video);

            CategoryDTO category;
            if (video.CategoryId.HasValue)
            {
                category = await _categoryService.GetByIdAsync(video.CategoryId.Value);
            }
            else
            {
                category = await _categoryService.GetByNameAsync(video.Category);
            }


            videoDTO.Category = category;

            var videoUpdated = await _videoService.UpdateVideoAsync(videoDTO);

            return Ok(_mapper.Map<VideoVM>(videoDTO));
        }

        [HttpGet]
        public async Task<IActionResult> Recommendations(string slug, int pageIndex, int pageSize)
        {
            if (pageIndex < 0 || pageSize < 1 || string.IsNullOrEmpty(slug))
                return BadRequest();

            var videoReference = await _videoService.FindBySlugAsync(slug);

            var recommendation = await _videoService.SearchRecommendationsAsync(pageIndex, pageSize, videoReference);

            IEnumerable<VideoCardVM> videosCards = _mapper.Map<IEnumerable<VideoCardVM>>(recommendation.Data);

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, recommendation.TotalCount.ToString());

            return Ok(videosCards);
        }

        [HttpGet]
        public async Task<IActionResult> GetThumbnail(string slug)
        {
            var video = await _videoService.FindBySlugAsync(slug);

            var path = AppContext.BaseDirectory;

            var filePath = Path.Combine(path, "..", "..", "..", "..", video.Thumbnail);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var contentType = GetContentType(filePath);
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetVideo(string slug)
        {
            var videoFile = await _videoService.FindBySlugAsync(slug);

            var filePath = Path.Combine($@"E:\ToDelete\{videoFile.Path}");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(fileStream, "video/mp4", enableRangeProcessing: true);
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }

        private bool GetSorting(string sort, out SortOption<SortVideo> sortOption)
        {
            var patternSort = @"^(title|update)_(ascending|descending)$";
            var regex = new Regex(patternSort);
            var match = regex.Match(sort);
            if (!match.Success)
            {
                sortOption = null;
                return false;
            }

            var field = match.Groups[1].Value;
            var direction = match.Groups[2].Value;
            sortOption = SortOptionFactory.Create<SortVideo>(field, direction);
            return true;
        }
    }
}
