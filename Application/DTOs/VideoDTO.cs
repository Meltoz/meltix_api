using Domain.Entities;

namespace Application.DTOs
{
    public class VideoDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public List<Tag> Tags { get; set; } = new List<Tag>();

        public CategoryDTO Category { get; set; }
        
    }
}
