namespace Web.ViewModels
{
    public class VideoVM
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public string Category { get; set; }

        public List<string> Tags { get; set; }

        public int Duration { get; set; }
    }
}
