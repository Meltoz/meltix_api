
using Shared;
using Shared.Exceptions;

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

        private readonly ICollection<Tag> _tags = new HashSet<Tag>();

        public IReadOnlyCollection<Tag> Tags => _tags.ToList();

        public Video() { }

        public Video(string path)
        {
            if (string.IsNullOrEmpty(path.Trim()))
                throw new ArgumentException("Path must be defined");

            Title = Path = path;
            Slug = SlugGenerator.Generate(path);
        }
        public Video(string path, string thumbnail) : this(path)
        {
            Thumbnail = thumbnail;
        }

        public Video(string path, string thumbnail, int duration) :this(path, thumbnail)
        {
            if (duration < 1)
                throw new ArgumentException("Duration must be postive");

            Duration = duration;
        }

        public void AddTags(Tag tag)
        {
            if (_tags.Contains(tag))
                throw new TagAlreadyExistException();

            _tags.Add(tag);
        }

        public void RemoveTags(Tag tag)
        {
            if (!_tags.Contains(tag))
                throw new EntityNotFoundException("Tag not found!");

            _tags.Remove(tag);
        }

        public void ChangeTitle(string title)
        {
            if (string.IsNullOrEmpty(title.Trim()))
                throw new ArgumentException("Title must be defined");

            Title = title;
            Slug = SlugGenerator.Generate(title);
        }
    }
}
