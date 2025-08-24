namespace Application.DTOs
{
    public class VideoDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;
        
    }
}
