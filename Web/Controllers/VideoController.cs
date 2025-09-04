using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Constants;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public VideoController(IVideoService vs, ICategoryService cs, IMapper m)
        {
            _videoService = vs;
            _categoryService = cs;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos(int pageIndex, int pageSize, string? search)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            var responseVideosPaginate = await _videoService.Paginate(pageIndex, pageSize, search ?? "");
            if (responseVideosPaginate.Status != Shared.ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }

            var videos = responseVideosPaginate.Response;
            IEnumerable<VideoCardVM> videoVM;
            try
            {
                videoVM = _mapper.Map<IEnumerable<VideoCardVM>>(videos.videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
            Response.Headers.Append(AppConstants.HeaderTotalCount, videos.totalCount.ToString());
            return Ok(videoVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetVideo(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest();
            }

            var responseFindVideo = await _videoService.FindBySlug(slug);
            
            if(responseFindVideo.Status == ServiceResponseStatus.Failure)
            {
                return StatusCode(500);
            }
            else if(responseFindVideo.Status  == ServiceResponseStatus.Warning)
            {
                return NotFound();
            }

            var videoDTO = responseFindVideo.Response;
            VideoVM video;

            try
            {
                video = _mapper.Map<VideoVM>(videoDTO);
            }
            catch(Exception ex)
            {
                return StatusCode(500);
            }

            return Ok(video);
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            await _videoService.SyncFolderWithDatabaseAsync();

            return Ok();
        }

        /// <summary>
        /// Obtain thumbnail of video
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetThumbnail(string slug)
        {
            var responseGetBySlug = await _videoService.FindBySlug(slug);

            if(responseGetBySlug == null || responseGetBySlug.Status == ServiceResponseStatus.Failure)
            {
                return StatusCode(500);
            }
            else if(responseGetBySlug.Status == ServiceResponseStatus.Warning)
            {
                return NotFound();
            }

            var video = responseGetBySlug.Response;

            var path = AppContext.BaseDirectory;
            
            var filePath = Path.Combine(path,"..", "..", "..", "..", video.Thumbnail);

            if (!System.IO.File.Exists(filePath))
                return NotFound();


            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);
            return File(bytes, contentType);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateVideo([FromForm] VideoRequestVM video)
        {
            // Checking
            if(!video.CategoryId.HasValue && string.IsNullOrEmpty(video.CategoryName))
            {
                return BadRequest();
            }

            if(video.Thumbnail is null && video.Img is null)
            {
                return BadRequest();
            }

            VideoDTO videoDTO;
            try
            {
                videoDTO = _mapper.Map<VideoDTO>(video);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            ServiceResponse<CategoryDTO> responseCategory;
            if (video.CategoryId.HasValue)
            {
                responseCategory =  await _categoryService.GetByIdAsync(video.CategoryId.Value);
            }
            else
            {
                responseCategory = await _categoryService.GetByNameAsync(video.CategoryName);
            }

            if (responseCategory.Status != ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }
            videoDTO.Category = responseCategory.Response;

            var responseUpdateVideo = await _videoService.UpdateVideo(videoDTO);
            if (responseUpdateVideo == null && responseUpdateVideo.Status != ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }
            var videoUpdated = responseUpdateVideo.Response;
            VideoVM videoReturned;
            try
            {
                videoReturned = _mapper.Map<VideoVM>(videoUpdated);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return Ok(videoReturned);
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
    }
}
