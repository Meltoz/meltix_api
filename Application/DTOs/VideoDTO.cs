namespace Application.DTOs
{
    public class VideoDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
        
        public int Duration { get; set; }

        public string Path { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();

        public CategoryDTO Category { get; set; }
        
    }
}
