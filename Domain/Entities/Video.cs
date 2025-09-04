
using Shared;

namespace Domain.Entities
{
    public class Video : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;

        public string Path { get; private set; } = string.Empty;

        public string Thumbnail { get; private set; } = string.Empty;

        public string Slug { get; private set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Duration { get; private set; }

        public Guid? CategoryId { get; set; }

        public Category? Category { get; set; }

        private readonly List<Tag> _tags = new();
        public IReadOnlyCollection<Tag> Tags => _tags;

        public Video() { }

        public Video(string path)
        {
            Title = Path = path;
            Slug = SlugGenerator.Generate(path);
        }
        public Video(string path, string thumbnail) : this(path)
        {
            Thumbnail = thumbnail;
        }

        public Video(string path, string thumbnail, int duration) :this(path, thumbnail)
        {
            Duration = duration;
        }

        public void AddTags(Tag tag)
        {
            if(!_tags.Contains(tag))
                _tags.Add(tag);
        }

        public void RemoveTags(Tag tag)
        {
            _tags.Remove(tag);
        }

        public void ChangeTitle(string title)
        {
            Title = title;
            Slug = SlugGenerator.Generate(title);
        }
    }
}
