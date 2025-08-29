using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IMapper _mapper;

        public VideoController(IVideoService vs, IMapper m)
        {
            _videoService = vs;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos(int pageIndex, int pageSize, string? search)
        {
            if(pageIndex < 0 || pageSize < 1)
                return BadRequest();

            var responseVideosPaginate = await _videoService.Paginate(pageIndex, pageSize, search ?? "");
            if(responseVideosPaginate.Status != Shared.ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }

            var videos = responseVideosPaginate.Response;
            IEnumerable<VideoVM> videoVM;
            try
            {
                videoVM = _mapper.Map<IEnumerable<VideoVM>>(videos.videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
            return Ok(new { videos = videoVM, totalCount= videos.totalCount });
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            await _videoService.SyncFolderWithDatabaseAsync();

            return Ok();
        }

    }
}
