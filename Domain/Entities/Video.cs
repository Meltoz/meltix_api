
namespace Domain.Entities
{
    public class Video : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;

        public string Path { get; private set; } = string.Empty;

        public string Thumbnail { get; private set; } = string.Empty;

        public ICollection<Category> Categories { get; private set; } = new List<Category>();
    }
}
