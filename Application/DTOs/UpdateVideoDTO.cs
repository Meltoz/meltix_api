using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class UpdateVideoDTO : VideoDTO
    {
        public TimeSpan? Timecode {  get; set; }

        public IFormFile? ThumbnailFile { get; set; }
    }
}
