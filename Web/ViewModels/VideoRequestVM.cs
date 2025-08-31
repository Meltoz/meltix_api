namespace Web.ViewModels
{
    public class VideoRequestVM
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public IFormFile? Img { get; set; }
        public string? Thumbnail { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
