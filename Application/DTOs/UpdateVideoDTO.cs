namespace Application.DTOs
{
    public class UpdateVideoDTO : VideoDTO
    {
        public TimeSpan? Timecode {  get; set; }

        public ThumbnailFileData? ThumbnailFile { get; set; }
    }
}
